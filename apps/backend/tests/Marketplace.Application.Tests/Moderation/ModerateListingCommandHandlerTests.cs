using Marketplace.Application.Moderation;
using Marketplace.Application.Tests.TestDoubles;
using Marketplace.Domain.Common;
using Marketplace.Domain.Listings;

namespace Marketplace.Application.Tests.Moderation;

public sealed class ModerateListingCommandHandlerTests
{
    private readonly FakeCurrentUser _admin = new() { UserId = Guid.NewGuid(), Role = UserRole.ADMIN };
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly StubClock _clock = new(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

    private ModerateListingCommandHandler Handler => new(_admin, _unitOfWork, _clock);

    private static Listing PendingListing() => Listing.Create(
        Guid.NewGuid(),
        "komatsu-pc200-abc",
        new ListingDetails(Guid.NewGuid(), null, "Komatsu PC200", Condition.USED, 100, false, "Hà Nội", "d", new ListingSpecs()),
        new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

    [Fact]
    public async Task Approve_publishes_the_listing_and_records_the_decision()
    {
        var listing = PendingListing();
        _unitOfWork.Listings.ToReturn = listing;

        await Handler.Handle(new ModerateListingCommand(listing.Id, ModerationAction.APPROVED, "looks good"), CancellationToken.None);

        Assert.Equal(ListingStatus.ACTIVE, listing.Status);
        var decision = Assert.Single(_unitOfWork.ModerationDecisions.Added);
        Assert.Equal(ModerationAction.APPROVED, decision.Decision);
        Assert.Equal(_admin.UserId, decision.AdminId);
        Assert.Equal(listing.Id, decision.ListingId);
        Assert.Equal(1, _unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Reject_marks_the_listing_rejected()
    {
        var listing = PendingListing();
        _unitOfWork.Listings.ToReturn = listing;

        await Handler.Handle(new ModerateListingCommand(listing.Id, ModerationAction.REJECTED, null), CancellationToken.None);

        Assert.Equal(ListingStatus.REJECTED, listing.Status);
        Assert.Equal(ModerationAction.REJECTED, Assert.Single(_unitOfWork.ModerationDecisions.Added).Decision);
    }

    [Fact]
    public async Task Throws_not_found_when_the_listing_is_missing()
    {
        _unitOfWork.Listings.ToReturn = null;

        var ex = await Assert.ThrowsAsync<NotFoundException>(
            () => Handler.Handle(new ModerateListingCommand(Guid.NewGuid(), ModerationAction.APPROVED, null), CancellationToken.None).AsTask());
        Assert.Equal(ErrorCodes.ListingNotFound, ex.Code);
        Assert.Empty(_unitOfWork.ModerationDecisions.Added);
    }

    [Fact]
    public async Task Requires_authentication()
    {
        _admin.UserId = null;
        _unitOfWork.Listings.ToReturn = PendingListing();

        await Assert.ThrowsAsync<UnauthenticatedException>(
            () => Handler.Handle(new ModerateListingCommand(Guid.NewGuid(), ModerationAction.APPROVED, null), CancellationToken.None).AsTask());
    }
}
