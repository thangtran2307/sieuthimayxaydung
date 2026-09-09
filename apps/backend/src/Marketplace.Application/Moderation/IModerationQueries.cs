namespace Marketplace.Application.Moderation;

/// <summary>Read side (Dapper) for the admin moderation views. Runs on the read connection.</summary>
public interface IModerationQueries
{
    /// <summary>Pending listings awaiting review, oldest first (FIFO queue).</summary>
    Task<IReadOnlyList<ModerationQueueItemDto>> GetPendingListingsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Open (unresolved) reports, oldest first.</summary>
    Task<IReadOnlyList<ReportDto>> GetOpenReportsAsync(CancellationToken cancellationToken = default);
}
