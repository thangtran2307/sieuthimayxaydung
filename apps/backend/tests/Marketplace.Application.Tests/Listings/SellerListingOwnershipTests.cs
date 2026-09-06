using Marketplace.Application.Listings;
using Marketplace.Application.Tests.TestDoubles;
using Marketplace.Domain.Common;
using Marketplace.Domain.Listings;

namespace Marketplace.Application.Tests.Listings;

/// <summary>Covers the shared ownership guard used by update / mark-sold / delete (FR-017).</summary>
public sealed class SellerListingOwnershipTests
{
    private readonly Guid _sellerId = Guid.NewGuid();
    private readonly FakeCurrentUser _currentUser;
    private readonly FakeCategoryQueries _categories = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly StubClock _clock = new(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
    private readonly Guid _categoryId;

    public SellerListingOwnershipTests()
    {
        _currentUser = new FakeCurrentUser { UserId = _sellerId };
        _categoryId = _categories.AddCategory();
    }

    private UpdateListingCommandHandler UpdateHandler => new(_currentUser, _categories, _unitOfWork, _clock);

    private Listing OwnedListing(Guid ownerId) => Listing.Create(
        ownerId,
        "komatsu-pc200-abc123",
        new ListingDetails(
            Guid.NewGuid(),
            null,
            "Komatsu PC200",
            Condition.USED,
            100_000_000,
            false,
            "Hà Nội",
            "Great condition.",
            new ListingSpecs()),
        _clock.UtcNow);

    private UpdateListingRequest UpdateBody() => new(
        _categoryId,
        null,
        "Updated",
        Condition.NEW,
        200_000_000,
        false,
        "Hà Nội",
        "Updated description.",
        null);

    [Fact]
    public async Task Update_edits_an_owned_listing()
    {
        var listing = OwnedListing(_sellerId);
        _unitOfWork.Listings.ToReturn = listing;

        await UpdateHandler.Handle(new UpdateListingCommand(listing.Id, UpdateBody()), CancellationToken.None);

        Assert.Equal("Updated", listing.Title);
        Assert.Equal(1, _unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Update_is_forbidden_for_a_listing_owned_by_someone_else()
    {
        _unitOfWork.Listings.ToReturn = OwnedListing(Guid.NewGuid()); // different owner

        var ex = await Assert.ThrowsAsync<ForbiddenException>(
            () => UpdateHandler.Handle(new UpdateListingCommand(Guid.NewGuid(), UpdateBody()), CancellationToken.None).AsTask());
        Assert.Equal(ErrorCodes.NotListingOwner, ex.Code);
        Assert.Equal(0, _unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Update_is_not_found_when_the_listing_is_missing()
    {
        _unitOfWork.Listings.ToReturn = null;

        var ex = await Assert.ThrowsAsync<NotFoundException>(
            () => UpdateHandler.Handle(new UpdateListingCommand(Guid.NewGuid(), UpdateBody()), CancellationToken.None).AsTask());
        Assert.Equal(ErrorCodes.ListingNotFound, ex.Code);
    }

    [Fact]
    public async Task MarkSold_transitions_an_owned_listing()
    {
        var listing = OwnedListing(_sellerId);
        _unitOfWork.Listings.ToReturn = listing;

        var handler = new MarkListingSoldCommandHandler(_currentUser, _unitOfWork, _clock);
        await handler.Handle(new MarkListingSoldCommand(listing.Id), CancellationToken.None);

        Assert.Equal(ListingStatus.SOLD, listing.Status);
    }

    [Fact]
    public async Task Delete_soft_deletes_an_owned_listing()
    {
        var listing = OwnedListing(_sellerId);
        _unitOfWork.Listings.ToReturn = listing;

        var handler = new DeleteListingCommandHandler(_currentUser, _unitOfWork, _clock);
        await handler.Handle(new DeleteListingCommand(listing.Id), CancellationToken.None);

        Assert.Equal(ListingStatus.REMOVED, listing.Status);
        Assert.Equal(1, _unitOfWork.SaveChangesCallCount);
    }
}
