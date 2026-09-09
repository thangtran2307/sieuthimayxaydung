using Marketplace.Application.Moderation;
using Marketplace.Application.Tests.TestDoubles;
using Marketplace.Domain.Common;
using Marketplace.Domain.Listings;

namespace Marketplace.Application.Tests.Moderation;

public sealed class BulkModerateListingsCommandHandlerTests
{
    private readonly FakeCurrentUser _admin = new() { UserId = Guid.NewGuid(), Role = UserRole.ADMIN };
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly StubClock _clock = new(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

    private BulkModerateListingsCommandHandler Handler => new(_admin, _unitOfWork, _clock);

    private static Listing PendingListing() => Listing.Create(
        Guid.NewGuid(),
        "komatsu-pc200-abc",
        new ListingDetails(Guid.NewGuid(), null, "Komatsu PC200", Condition.USED, 100, false, "Hà Nội", "d", new ListingSpecs()),
        new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

    [Fact]
    public async Task Approves_pending_listings_and_reports_the_count()
    {
        var one = PendingListing();
        var two = PendingListing();
        _unitOfWork.Listings.Items.AddRange([one, two]);

        var result = await Handler.Handle(
            new BulkModerateListingsCommand(new BulkModerateRequest([one.Id, two.Id], ModerationAction.APPROVED, null)),
            CancellationToken.None);

        Assert.Equal(2, result.Succeeded);
        Assert.Equal(0, result.Skipped);
        Assert.Equal(ListingStatus.ACTIVE, one.Status);
        Assert.Equal(2, _unitOfWork.ModerationDecisions.Added.Count);
        Assert.Equal(1, _unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Counts_missing_ids_as_skipped()
    {
        var one = PendingListing();
        _unitOfWork.Listings.Items.Add(one);

        var result = await Handler.Handle(
            new BulkModerateListingsCommand(
                new BulkModerateRequest([one.Id, Guid.NewGuid(), Guid.NewGuid()], ModerationAction.APPROVED, null)),
            CancellationToken.None);

        Assert.Equal(1, result.Succeeded);
        Assert.Equal(2, result.Skipped);
        Assert.Single(_unitOfWork.ModerationDecisions.Added);
    }

    [Fact]
    public async Task Skips_listings_in_an_incompatible_state()
    {
        var pending = PendingListing();
        var alreadyActive = PendingListing();
        alreadyActive.Approve(_clock.UtcNow); // no longer pending → approve will be skipped
        _unitOfWork.Listings.Items.AddRange([pending, alreadyActive]);

        var result = await Handler.Handle(
            new BulkModerateListingsCommand(
                new BulkModerateRequest([pending.Id, alreadyActive.Id], ModerationAction.APPROVED, null)),
            CancellationToken.None);

        Assert.Equal(1, result.Succeeded);
        Assert.Equal(1, result.Skipped);
    }
}
