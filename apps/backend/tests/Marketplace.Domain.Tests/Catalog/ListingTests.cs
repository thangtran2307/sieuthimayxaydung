using Marketplace.Domain.Catalog;

namespace Marketplace.Domain.Tests.Catalog;

public sealed class ListingTests
{
    [Fact]
    public void RegisterView_increments_the_view_count()
    {
        var listing = new Listing();

        listing.RegisterView();
        listing.RegisterView();

        Assert.Equal(2, listing.ViewCount);
    }
}
