using Marketplace.Application.Listings;
using Marketplace.Application.Tests.TestDoubles;
using Marketplace.Domain.Common;

namespace Marketplace.Application.Tests.Listings;

public sealed class CreateListingCommandHandlerTests
{
    private readonly FakeCurrentUser _currentUser = new() { UserId = Guid.NewGuid() };
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly StubClock _clock = new(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

    private CreateListingCommandHandler Handler => new(_currentUser, _unitOfWork, _clock);

    private static CreateListingRequest Request() => new(
        Guid.NewGuid(),
        null,
        "Máy đào Komatsu PC200",
        Condition.USED,
        850_000_000,
        false,
        "Hà Nội",
        "Well maintained.",
        new ListingSpecsInput(2018, "Komatsu", "PC200", 5000, "Japan", "20t"),
        [new ListingPhotoInput("https://cdn/1.jpg", 0, 800, 600)]);

    [Fact]
    public async Task Creates_a_pending_listing_owned_by_the_current_seller()
    {
        var result = await Handler.Handle(new CreateListingCommand(Request()), CancellationToken.None);

        var saved = Assert.Single(_unitOfWork.Listings.Added);
        Assert.Equal(_currentUser.UserId, saved.SellerId);
        Assert.Equal(ListingStatus.PENDING, saved.Status);
        Assert.Single(saved.Photos);
        Assert.StartsWith("may-dao-komatsu-pc200-", saved.Slug);
        Assert.Equal(1, _unitOfWork.SaveChangesCallCount);

        Assert.Equal(saved.Id, result.Id);
        Assert.Equal(ListingStatus.PENDING, result.Status);
    }

    [Fact]
    public async Task Requires_an_authenticated_seller()
    {
        _currentUser.UserId = null;

        await Assert.ThrowsAsync<UnauthenticatedException>(
            () => Handler.Handle(new CreateListingCommand(Request()), CancellationToken.None).AsTask());
    }
}
