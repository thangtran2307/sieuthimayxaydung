using Mediator;

namespace Marketplace.Application.Moderation;

/// <summary>Returns the pending-listing moderation queue for an admin (FR-019).</summary>
public sealed record GetModerationQueueQuery : IQuery<IReadOnlyList<ModerationQueueItemDto>>;

public sealed class GetModerationQueueQueryHandler(IModerationQueries moderationQueries)
    : IQueryHandler<GetModerationQueueQuery, IReadOnlyList<ModerationQueueItemDto>>
{
    public async ValueTask<IReadOnlyList<ModerationQueueItemDto>> Handle(
        GetModerationQueueQuery query,
        CancellationToken cancellationToken) =>
        await moderationQueries.GetPendingListingsAsync(cancellationToken);
}
