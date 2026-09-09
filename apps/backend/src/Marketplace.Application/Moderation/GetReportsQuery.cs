using Mediator;

namespace Marketplace.Application.Moderation;

/// <summary>Returns the open (unresolved) reports for an admin to review (FR-021).</summary>
public sealed record GetReportsQuery : IQuery<IReadOnlyList<ReportDto>>;

public sealed class GetReportsQueryHandler(IModerationQueries moderationQueries)
    : IQueryHandler<GetReportsQuery, IReadOnlyList<ReportDto>>
{
    public async ValueTask<IReadOnlyList<ReportDto>> Handle(
        GetReportsQuery query,
        CancellationToken cancellationToken) =>
        await moderationQueries.GetOpenReportsAsync(cancellationToken);
}
