using System.Data;
using System.Text.Json;
using Dapper;
using Marketplace.Application.Categories;
using Marketplace.Application.Common.Models;
using Marketplace.Application.Listings;

namespace Marketplace.Infrastructure.Persistence.Queries;

/// <summary>
/// Dapper read side for listings — search, public detail, and contact lookup. Runs on the read
/// connection. Dynamic search SQL is composed with Dapper.SqlBuilder so every value is
/// parameterized (no hand-concatenated user input).
/// </summary>
internal sealed class ListingQueries(ReadDbConnectionFactory connectionFactory, IClock clock)
    : IListingQueries
{
    private const string SearchFrom = """
        FROM listings l
        JOIN categories c ON c.id = l.category_id
        LEFT JOIN categories sc ON sc.id = l.subcategory_id
        """;

    // Highest-priority currently-active boost for each listing (drives the "boosted first" ranking).
    private const string ActiveBoostJoin = """
        LEFT JOIN LATERAL (
            SELECT bo.priority_level
            FROM boosts bo
            WHERE bo.listing_id = l.id AND bo.status = 'ACTIVE'
              AND (bo.starts_at IS NULL OR bo.starts_at <= @now)
              AND (bo.expires_at IS NULL OR bo.expires_at > @now)
            ORDER BY bo.priority_level DESC
            LIMIT 1
        ) b ON TRUE
        """;

    private static readonly IReadOnlyDictionary<string, JsonElement> EmptySpecs =
        new Dictionary<string, JsonElement>();

    public async Task<PagedResult<ListingSummaryDto>> SearchAsync(
        ListingSearchCriteria criteria,
        CancellationToken cancellationToken = default)
    {
        var builder = new SqlBuilder();
        builder.Where("l.status = 'ACTIVE'");

        if (criteria.CategorySlug is not null)
        {
            builder.Where("c.slug = @category", new { category = criteria.CategorySlug });
        }

        if (criteria.SubcategorySlug is not null)
        {
            builder.Where("sc.slug = @subcategory", new { subcategory = criteria.SubcategorySlug });
        }

        if (criteria.Condition is not null)
        {
            builder.Where("l.condition = @condition", new { condition = criteria.Condition.Value.ToString() });
        }

        if (criteria.Province is not null)
        {
            builder.Where("l.location_province = @province", new { province = criteria.Province });
        }

        // Price filters exclude "contact for price" rows (they have no comparable amount).
        if (criteria.PriceMin is not null)
        {
            builder.Where(
                "l.price_contact = FALSE AND l.price_amount >= @priceMin",
                new { priceMin = criteria.PriceMin.Value });
        }

        if (criteria.PriceMax is not null)
        {
            builder.Where(
                "l.price_contact = FALSE AND l.price_amount <= @priceMax",
                new { priceMax = criteria.PriceMax.Value });
        }

        if (criteria.Query is not null)
        {
            builder.Where(
                "l.search_vector @@ websearch_to_tsquery('simple', @q)",
                new { q = criteria.Query });
        }

        // Boosted listings first, then the requested sort.
        builder.OrderBy("(b.priority_level IS NOT NULL) DESC");
        builder.OrderBy("b.priority_level DESC NULLS LAST");
        builder.OrderBy(OrderByClause(criteria.Sort));

        var countTemplate = builder.AddTemplate($"SELECT COUNT(*) {SearchFrom} /**where**/");

        using var connection = connectionFactory.Create();

        int total = await connection.ExecuteScalarAsync<int>(new CommandDefinition(
            countTemplate.RawSql,
            countTemplate.Parameters,
            cancellationToken: cancellationToken));

        var items = new List<ListingSummaryDto>();
        if (total > 0)
        {
            var pageTemplate = builder.AddTemplate(
                $"""
                SELECT
                    l.id, l.slug, l.title, l.price_amount, l.price_contact, l.currency,
                    l.condition, l.location_province,
                    (SELECT p.url FROM listing_photos p
                     WHERE p.listing_id = l.id ORDER BY p.sort_order LIMIT 1) AS thumbnail_url,
                    (b.priority_level IS NOT NULL) AS boosted,
                    c.slug AS category_slug,
                    l.created_at
                {SearchFrom}
                {ActiveBoostJoin}
                /**where**/
                /**orderby**/
                LIMIT @take OFFSET @skip
                """,
                new
                {
                    now = clock.UtcNow,
                    take = criteria.PageSize,
                    skip = (criteria.Page - 1) * criteria.PageSize,
                });

            var rows = await connection.QueryAsync<ListingSummaryDto>(new CommandDefinition(
                pageTemplate.RawSql,
                pageTemplate.Parameters,
                cancellationToken: cancellationToken));
            items = rows.AsList();
        }

        return PagedResult<ListingSummaryDto>.Create(items, total, criteria.Page, criteria.PageSize);
    }

    public async Task<ListingDetailDto> GetBySlugAsync(
        string slug,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                l.id, l.slug, l.title, l.condition, l.price_amount, l.price_contact, l.currency,
                l.location_province, l.description, l.specs::text AS specs_json, l.status,
                l.view_count, l.created_at, l.published_at,
                l.category_id, l.subcategory_id, l.seller_id,
                EXISTS (
                    SELECT 1 FROM boosts bo
                    WHERE bo.listing_id = l.id AND bo.status = 'ACTIVE'
                      AND (bo.starts_at IS NULL OR bo.starts_at <= @now)
                      AND (bo.expires_at IS NULL OR bo.expires_at > @now)
                ) AS boosted
            FROM listings l
            WHERE l.slug = @slug AND l.status = 'ACTIVE'
            """;

        using var connection = connectionFactory.Create();

        var row = await connection.QuerySingleOrDefaultAsync<ListingDetailRow>(new CommandDefinition(
            sql,
            new { slug, now = clock.UtcNow },
            cancellationToken: cancellationToken));

        if (row is null)
        {
            return null;
        }

        var category = await GetCategoryAsync(connection, row.CategoryId, cancellationToken);
        var subcategory = row.SubcategoryId is null
            ? null
            : await GetCategoryAsync(connection, row.SubcategoryId.Value, cancellationToken);
        var seller = await GetSellerAsync(connection, row.SellerId, cancellationToken);
        var photos = await GetPhotosAsync(connection, row.Id, cancellationToken);

        return new ListingDetailDto(
            row.Id,
            row.Slug,
            row.Title,
            row.Condition,
            row.PriceAmount,
            row.PriceContact,
            row.Currency,
            row.LocationProvince,
            row.Description,
            ParseSpecs(row.SpecsJson),
            photos,
            row.Status,
            row.Boosted,
            category,
            subcategory,
            seller,
            row.ViewCount,
            row.CreatedAt,
            row.PublishedAt);
    }

    public async Task<IReadOnlyList<SellerListingDto>> GetSellerListingsAsync(
        Guid sellerId,
        CancellationToken cancellationToken = default)
    {
        // Column order MUST match the SellerListingDto constructor — Dapper binds a positional record
        // by parameter position, not by name.
        const string sql = """
            SELECT
                l.id, l.slug, l.title, l.status, l.price_amount, l.price_contact, l.currency,
                l.view_count,
                (SELECT p.url FROM listing_photos p
                 WHERE p.listing_id = l.id ORDER BY p.sort_order LIMIT 1) AS thumbnail_url,
                l.created_at, l.published_at
            FROM listings l
            WHERE l.seller_id = @sellerId AND l.status <> 'REMOVED'
            ORDER BY l.created_at DESC
            """;

        using var connection = connectionFactory.Create();
        var rows = await connection.QueryAsync<SellerListingDto>(new CommandDefinition(
            sql,
            new { sellerId },
            cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<SellerListingDetailDto> GetSellerListingAsync(
        Guid sellerId,
        Guid listingId,
        CancellationToken cancellationToken = default)
    {
        // Scoped to the owner: a listing that is missing or owned by someone else returns null.
        const string sql = """
            SELECT
                l.id, l.category_id, l.subcategory_id, l.title, l.condition,
                l.price_amount, l.price_contact, l.location_province, l.description,
                l.specs::text AS specs_json, l.status
            FROM listings l
            WHERE l.id = @listingId AND l.seller_id = @sellerId
            """;

        using var connection = connectionFactory.Create();
        var row = await connection.QuerySingleOrDefaultAsync<SellerListingDetailRow>(new CommandDefinition(
            sql,
            new { sellerId, listingId },
            cancellationToken: cancellationToken));

        return row is null
            ? null
            : new SellerListingDetailDto(
            row.Id,
            row.CategoryId,
            row.SubcategoryId,
            row.Title,
            row.Condition,
            row.PriceAmount,
            row.PriceContact,
            row.LocationProvince,
            row.Description,
            ParseSpecsDto(row.SpecsJson),
            row.Status);
    }

    public async Task<ListingContact> GetListingContactAsync(
        Guid listingId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT l.id AS listing_id, l.seller_id, l.status, u.phone AS seller_phone
            FROM listings l
            JOIN users u ON u.id = l.seller_id
            WHERE l.id = @id
            """;

        using var connection = connectionFactory.Create();
        return await connection.QuerySingleOrDefaultAsync<ListingContact>(new CommandDefinition(
            sql,
            new { id = listingId },
            cancellationToken: cancellationToken));
    }

    private static string OrderByClause(string sort) => sort switch
    {
        "price_asc" => "l.price_amount ASC NULLS LAST",
        "price_desc" => "l.price_amount DESC NULLS LAST",
        "relevance" => "ts_rank(l.search_vector, websearch_to_tsquery('simple', @q)) DESC",
        _ => "l.created_at DESC",
    };

    private static IReadOnlyDictionary<string, JsonElement> ParseSpecs(string specsJson)
    {
        if (string.IsNullOrWhiteSpace(specsJson))
        {
            return EmptySpecs;
        }

        var parsed = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(specsJson);
        if (parsed is null)
        {
            return EmptySpecs;
        }

        // Drop null-valued keys so the API only exposes populated specifications.
        return parsed
            .Where(kvp => kvp.Value.ValueKind is not JsonValueKind.Null)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    // Case-insensitive so the parse is robust to the JSON key casing EF uses for the owned Specs type.
    private static readonly JsonSerializerOptions SpecsDtoOptions = new() { PropertyNameCaseInsensitive = true };

    private static ListingSpecsDto ParseSpecsDto(string specsJson) =>
        string.IsNullOrWhiteSpace(specsJson)
            ? new ListingSpecsDto(null, null, null, null, null, null)
            : JsonSerializer.Deserialize<ListingSpecsDto>(specsJson, SpecsDtoOptions)
              ?? new ListingSpecsDto(null, null, null, null, null, null);

    private static async Task<CategoryDto> GetCategoryAsync(
        IDbConnection connection,
        Guid id,
        CancellationToken cancellationToken) =>
        await connection.QuerySingleOrDefaultAsync<CategoryDto>(new CommandDefinition(
            "SELECT id, slug, parent_id, label_vi, label_en, icon, sort_order FROM categories WHERE id = @id",
            new { id },
            cancellationToken: cancellationToken));

    private static async Task<PublicSellerDto> GetSellerAsync(
        IDbConnection connection,
        Guid id,
        CancellationToken cancellationToken) =>
        await connection.QuerySingleOrDefaultAsync<PublicSellerDto>(new CommandDefinition(
            "SELECT id, display_name, location_province, verified, created_at AS joined_at FROM users WHERE id = @id",
            new { id },
            cancellationToken: cancellationToken));

    private static async Task<IReadOnlyList<ListingPhotoDto>> GetPhotosAsync(
        IDbConnection connection,
        Guid listingId,
        CancellationToken cancellationToken)
    {
        var rows = await connection.QueryAsync<ListingPhotoDto>(new CommandDefinition(
            "SELECT url, sort_order, width, height FROM listing_photos WHERE listing_id = @id ORDER BY sort_order",
            new { id = listingId },
            cancellationToken: cancellationToken));
        return rows.AsList();
    }

    /// <summary>
    /// Flat projection of the listing detail row before related entities are attached. Positional
    /// record — the parameter order MUST match the GetBySlugAsync SELECT (Dapper binds by position).
    /// </summary>
    private sealed record ListingDetailRow(
        Guid Id,
        string Slug,
        string Title,
        Condition Condition,
        long? PriceAmount,
        bool PriceContact,
        string Currency,
        string LocationProvince,
        string Description,
        string SpecsJson,
        ListingStatus Status,
        int ViewCount,
        DateTime CreatedAt,
        DateTime? PublishedAt,
        Guid CategoryId,
        Guid? SubcategoryId,
        Guid SellerId,
        bool Boosted);

    /// <summary>
    /// Flat projection of a seller's own listing for the edit form. Positional record — the parameter
    /// order MUST match the GetSellerListingAsync SELECT (Dapper binds by position).
    /// </summary>
    private sealed record SellerListingDetailRow(
        Guid Id,
        Guid CategoryId,
        Guid? SubcategoryId,
        string Title,
        Condition Condition,
        long? PriceAmount,
        bool PriceContact,
        string LocationProvince,
        string Description,
        string SpecsJson,
        ListingStatus Status);
}
