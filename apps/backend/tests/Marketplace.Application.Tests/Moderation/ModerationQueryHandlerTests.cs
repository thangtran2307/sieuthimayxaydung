using Marketplace.Application.Moderation;
using Marketplace.Application.Tests.TestDoubles;
using Marketplace.Domain.Common;

namespace Marketplace.Application.Tests.Moderation;

public sealed class ModerationQueryHandlerTests
{
    private readonly FakeModerationQueries _queries = new();

    [Fact]
    public async Task Queue_returns_pending_listings()
    {
        _queries.PendingResult =
        [
            new ModerationQueueItemDto(
                Guid.NewGuid(), "slug", "Title", "Seller", "excavators", 100, false, "VND", "Hà Nội", DateTime.UtcNow),
        ];

        var result = await new GetModerationQueueQueryHandler(_queries)
            .Handle(new GetModerationQueueQuery(), CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task Reports_returns_open_reports()
    {
        _queries.ReportsResult =
        [
            new ReportDto(
                Guid.NewGuid(), Guid.NewGuid(), "slug", "Title", ListingStatus.ACTIVE,
                ReportReason.SPAM, null, null, ReportStatus.OPEN, DateTime.UtcNow),
        ];

        var result = await new GetReportsQueryHandler(_queries)
            .Handle(new GetReportsQuery(), CancellationToken.None);

        Assert.Single(result);
    }
}

public sealed class ModerationValidatorTests
{
    [Fact]
    public void Bulk_request_requires_at_least_one_listing()
    {
        var validator = new BulkModerateRequestValidator();

        Assert.False(validator.Validate(new BulkModerateRequest([], ModerationAction.APPROVED, null)).IsValid);
        Assert.True(validator.Validate(new BulkModerateRequest([Guid.NewGuid()], ModerationAction.APPROVED, null)).IsValid);
    }

    [Fact]
    public void Resolve_request_validates_the_resolution_enum()
    {
        var validator = new ResolveReportRequestValidator();

        Assert.True(validator.Validate(new ResolveReportRequest(ReportResolution.REMOVE, "reason")).IsValid);
        Assert.False(validator.Validate(new ResolveReportRequest((ReportResolution)99, null)).IsValid);
    }
}
