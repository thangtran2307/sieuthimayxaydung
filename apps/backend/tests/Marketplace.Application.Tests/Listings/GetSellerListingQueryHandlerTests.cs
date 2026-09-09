using Marketplace.Application.Listings;
using Marketplace.Application.Tests.TestDoubles;
using Marketplace.Domain.Common;

namespace Marketplace.Application.Tests.Listings;

public sealed class GetSellerListingQueryHandlerTests
{
    private readonly FakeCurrentUser _currentUser = new() { UserId = Guid.NewGuid() };
    private readonly FakeListingQueries _queries = new();

    private GetSellerListingQueryHandler Handler => new(_currentUser, _queries);

    private static SellerListingDetailDto Detail(Guid id) => new(
        id,
        Guid.NewGuid(),
        null,
        "Komatsu PC200",
        Condition.USED,
        850_000_000,
        false,
        "Hà Nội",
        "Great condition.",
        new ListingSpecsDto(2018, "Komatsu", null, null, null, null),
        ListingStatus.ACTIVE);

    [Fact]
    public async Task Returns_the_editable_state_scoped_to_the_current_seller()
    {
        var id = Guid.NewGuid();
        _queries.SellerListingResult = Detail(id);

        var result = await Handler.Handle(new GetSellerListingQuery(id), CancellationToken.None);

        Assert.Equal(id, result.Id);
        Assert.Equal("Komatsu PC200", result.Title);
        Assert.Equal(2018, result.Specs.Year);
        Assert.Equal(_currentUser.UserId, _queries.CapturedSellerId);
    }

    [Fact]
    public async Task Throws_not_found_when_missing_or_not_owned()
    {
        _queries.SellerListingResult = null;

        var ex = await Assert.ThrowsAsync<NotFoundException>(
            () => Handler.Handle(new GetSellerListingQuery(Guid.NewGuid()), CancellationToken.None).AsTask());
        Assert.Equal(ErrorCodes.ListingNotFound, ex.Code);
    }

    [Fact]
    public async Task Requires_authentication()
    {
        _currentUser.UserId = null;

        await Assert.ThrowsAsync<UnauthenticatedException>(
            () => Handler.Handle(new GetSellerListingQuery(Guid.NewGuid()), CancellationToken.None).AsTask());
    }
}
