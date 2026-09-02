using Marketplace.Application.Listings;
using Marketplace.Application.Tests.TestDoubles;
using Marketplace.Domain.Common;

namespace Marketplace.Application.Tests.Listings;

public sealed class GetMyListingsQueryHandlerTests
{
    private readonly FakeCurrentUser _currentUser = new() { UserId = Guid.NewGuid() };
    private readonly FakeListingQueries _queries = new();

    private GetMyListingsQueryHandler Handler => new(_currentUser, _queries);

    [Fact]
    public async Task Returns_the_current_sellers_listings()
    {
        _queries.SellerListingsResult =
        [
            new SellerListingDto(
                Guid.NewGuid(), "komatsu-pc200-abc123", "Komatsu PC200", ListingStatus.PENDING,
                850_000_000, false, "VND", 0, null, DateTime.UtcNow, null),
        ];

        var result = await Handler.Handle(new GetMyListingsQuery(), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(_currentUser.UserId, _queries.CapturedSellerId);
    }

    [Fact]
    public async Task Requires_authentication()
    {
        _currentUser.UserId = null;

        await Assert.ThrowsAsync<UnauthenticatedException>(
            () => Handler.Handle(new GetMyListingsQuery(), CancellationToken.None).AsTask());
    }
}
