using Marketplace.Application.Moderation;
using Marketplace.Application.Tests.TestDoubles;
using Marketplace.Domain.Common;
using Marketplace.Domain.Listings;
using Marketplace.Domain.Reports;

namespace Marketplace.Application.Tests.Moderation;

public sealed class ResolveReportCommandHandlerTests
{
    private readonly FakeCurrentUser _admin = new() { UserId = Guid.NewGuid(), Role = UserRole.ADMIN };
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly StubClock _clock = new(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

    private ResolveReportCommandHandler Handler => new(_admin, _unitOfWork, _clock);

    private static Listing ActiveListing()
    {
        var listing = Listing.Create(
            Guid.NewGuid(),
            "komatsu-pc200-abc",
            new ListingDetails(Guid.NewGuid(), null, "Komatsu PC200", Condition.USED, 100, false, "Hà Nội", "d", new ListingSpecs()),
            new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        listing.Approve(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        return listing;
    }

    [Fact]
    public async Task Remove_resolution_removes_the_listing_and_resolves_the_report()
    {
        var report = Report.Create(Guid.NewGuid(), ReportReason.FRAUD, _clock.UtcNow);
        var listing = ActiveListing();
        _unitOfWork.Reports.ToReturn = report;
        _unitOfWork.Listings.ToReturn = listing;

        await Handler.Handle(
            new ResolveReportCommand(report.Id, new ResolveReportRequest(ReportResolution.REMOVE, "fraudulent")),
            CancellationToken.None);

        Assert.Equal(ListingStatus.REMOVED, listing.Status);
        Assert.Equal(ReportStatus.RESOLVED_REMOVED, report.Status);
        Assert.Equal(_admin.UserId, report.ResolvedByAdminId);
        Assert.Equal(ModerationAction.REMOVED, Assert.Single(_unitOfWork.ModerationDecisions.Added).Decision);
        Assert.Equal(1, _unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Clear_resolution_resolves_without_touching_the_listing()
    {
        var report = Report.Create(Guid.NewGuid(), ReportReason.OTHER, _clock.UtcNow);
        _unitOfWork.Reports.ToReturn = report;

        await Handler.Handle(
            new ResolveReportCommand(report.Id, new ResolveReportRequest(ReportResolution.CLEAR, null)),
            CancellationToken.None);

        Assert.Equal(ReportStatus.RESOLVED_CLEARED, report.Status);
        Assert.Empty(_unitOfWork.ModerationDecisions.Added);
    }

    [Fact]
    public async Task Throws_not_found_when_the_report_is_missing()
    {
        _unitOfWork.Reports.ToReturn = null;

        await Assert.ThrowsAsync<NotFoundException>(
            () => Handler.Handle(
                new ResolveReportCommand(Guid.NewGuid(), new ResolveReportRequest(ReportResolution.CLEAR, null)),
                CancellationToken.None).AsTask());
    }
}
