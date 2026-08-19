using Marketplace.Application.Listings;
using Marketplace.Application.Tests.TestDoubles;
using Marketplace.Domain.Common;
using Marketplace.Domain.Listings;

namespace Marketplace.Application.Tests.Listings;

public sealed class GetListingBySlugQueryHandlerTests
{
    private readonly FakeListingQueries _queries = new();
    private readonly FakeUnitOfWork _unitOfWork = new();

    private GetListingBySlugQueryHandler Handler => new(_queries, _unitOfWork);

    [Fact]
    public async Task Throws_not_found_when_listing_is_not_public()
    {
        _queries.DetailResult = null;

        var ex = await Assert.ThrowsAsync<NotFoundException>(
            () => Handler.Handle(new GetListingBySlugQuery("missing"), CancellationToken.None).AsTask());

        Assert.Equal(ErrorCodes.ListingNotFound, ex.Code);
    }

    [Fact]
    public async Task Records_a_view_and_returns_the_incremented_count()
    {
        var id = Guid.NewGuid();
        _queries.DetailResult = Fixtures.Detail(id, viewCount: 5);
        _unitOfWork.Listings.ToReturn = new Listing();

        var result = await Handler.Handle(new GetListingBySlugQuery("excavator-abc123"), CancellationToken.None);

        Assert.Equal(6, result.ViewCount);                         // reflected in the response
        Assert.Equal(1, _unitOfWork.Listings.ToReturn.ViewCount);  // persisted via the aggregate
        Assert.Equal(1, _unitOfWork.SaveChangesCallCount);
    }
}
