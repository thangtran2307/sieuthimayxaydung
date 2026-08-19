using System.Text.Json;
using Marketplace.Application.Categories;

namespace Marketplace.Application.Listings;

/// <summary>Card shape for search/listing rows (mirrors the OpenAPI <c>ListingSummary</c>).</summary>
public sealed record ListingSummaryDto(
    Guid Id,
    string Slug,
    string Title,
    long? PriceAmount,
    bool PriceContact,
    string Currency,
    Condition Condition,
    string LocationProvince,
    string ThumbnailUrl,
    bool Boosted,
    string CategorySlug,
    DateTime CreatedAt);

/// <summary>A single photo on a listing (gallery order via <see cref="SortOrder"/>; 0 = primary).</summary>
public sealed record ListingPhotoDto(string Url, int SortOrder, int? Width, int? Height);

/// <summary>Seller information safe for public exposure (never credentials).</summary>
public sealed record PublicSellerDto(
    Guid Id,
    string DisplayName,
    string LocationProvince,
    bool Verified,
    DateTime JoinedAt);

/// <summary>Full public listing detail (mirrors the OpenAPI <c>Listing</c>).</summary>
public sealed record ListingDetailDto(
    Guid Id,
    string Slug,
    string Title,
    Condition Condition,
    long? PriceAmount,
    bool PriceContact,
    string Currency,
    string LocationProvince,
    string Description,
    IReadOnlyDictionary<string, JsonElement> Specs,
    IReadOnlyList<ListingPhotoDto> Photos,
    ListingStatus Status,
    bool Boosted,
    CategoryDto Category,
    CategoryDto Subcategory,
    PublicSellerDto Seller,
    int ViewCount,
    DateTime CreatedAt,
    DateTime? PublishedAt);

/// <summary>
/// Normalized (clamped) criteria passed to the read side for a listing search. Only ACTIVE listings
/// are returned; boosted listings are always ranked first (see the Dapper query).
/// </summary>
public sealed record ListingSearchCriteria(
    string Query,
    string CategorySlug,
    string SubcategorySlug,
    Condition? Condition,
    string Province,
    long? PriceMin,
    long? PriceMax,
    string Sort,
    int Page,
    int PageSize);

/// <summary>Contact details needed to record an inquiry against a listing.</summary>
public sealed record ListingContact(Guid ListingId, Guid SellerId, ListingStatus Status, string SellerPhone);
