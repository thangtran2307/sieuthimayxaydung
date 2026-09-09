using Dapper;
using Marketplace.Application.Moderation;

namespace Marketplace.Infrastructure.Persistence.Queries;

/// <summary>Dapper read side for the admin moderation views (runs on the read connection).</summary>
internal sealed class ModerationQueries(ReadDbConnectionFactory connectionFactory) : IModerationQueries
{
    public async Task<IReadOnlyList<ModerationQueueItemDto>> GetPendingListingsAsync(
        CancellationToken cancellationToken = default)
    {
        // Column order matches the ModerationQueueItemDto constructor (Dapper binds positional records by position).
        const string sql = """
            SELECT
                l.id, l.slug, l.title, u.display_name AS seller_name, c.slug AS category_slug,
                l.price_amount, l.price_contact, l.currency, l.location_province, l.created_at
            FROM listings l
            JOIN users u ON u.id = l.seller_id
            JOIN categories c ON c.id = l.category_id
            WHERE l.status = 'PENDING'
            ORDER BY l.created_at ASC
            """;

        using var connection = connectionFactory.Create();
        var rows = await connection.QueryAsync<ModerationQueueItemDto>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<IReadOnlyList<ReportDto>> GetOpenReportsAsync(
        CancellationToken cancellationToken = default)
    {
        // Column order matches the ReportDto constructor (Dapper binds positional records by position).
        const string sql = """
            SELECT
                r.id, r.listing_id, l.slug AS listing_slug, l.title AS listing_title, l.status AS listing_status,
                r.reason, r.details, r.reporter_contact, r.status, r.created_at
            FROM reports r
            JOIN listings l ON l.id = r.listing_id
            WHERE r.status = 'OPEN'
            ORDER BY r.created_at ASC
            """;

        using var connection = connectionFactory.Create();
        var rows = await connection.QueryAsync<ReportDto>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
        return rows.AsList();
    }
}
