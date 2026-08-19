using Marketplace.Application.Listings;
using Marketplace.Application.Reports;
using Marketplace.Application.Tests.TestDoubles;
using Marketplace.Domain.Common;

namespace Marketplace.Application.Tests.Reports;

public sealed class CreateReportCommandHandlerTests
{
    private readonly FakeListingQueries _queries = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly StubClock _clock = new(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

    private CreateReportCommandHandler Handler => new(_queries, _unitOfWork, _clock);

    [Fact]
    public async Task Throws_not_found_when_listing_is_missing()
    {
        _queries.ContactResult = null;

        var command = new CreateReportCommand(
            Guid.NewGuid(),
            new CreateReportRequest(ReportReason.FRAUD, null, null));

        var ex = await Assert.ThrowsAsync<NotFoundException>(
            () => Handler.Handle(command, CancellationToken.None).AsTask());
        Assert.Equal(ErrorCodes.ListingNotFound, ex.Code);
    }

    [Fact]
    public async Task Records_the_report_when_the_listing_exists()
    {
        var listingId = Guid.NewGuid();
        _queries.ContactResult = new ListingContact(listingId, Guid.NewGuid(), ListingStatus.ACTIVE, "0900000000");

        var command = new CreateReportCommand(
            listingId,
            new CreateReportRequest(ReportReason.SPAM, "Duplicate posting", "reporter@example.com"));

        await Handler.Handle(command, CancellationToken.None);

        var recorded = Assert.Single(_unitOfWork.Reports.Added);
        Assert.Equal(listingId, recorded.ListingId);
        Assert.Equal(ReportReason.SPAM, recorded.Reason);
        Assert.Equal(ReportStatus.OPEN, recorded.Status);
        Assert.Equal(1, _unitOfWork.SaveChangesCallCount);
    }
}
