using Marketplace.Application.Catalog;
using Marketplace.Application.Tests.TestDoubles;

namespace Marketplace.Application.Tests.Catalog;

public sealed class SearchListingsQueryHandlerTests
{
    private readonly FakeListingQueries _queries = new();

    private async Task<ListingSearchCriteria> HandleAsync(SearchListingsQuery query)
    {
        var handler = new SearchListingsQueryHandler(_queries);
        await handler.Handle(query, CancellationToken.None);
        return _queries.CapturedCriteria;
    }

    [Fact]
    public async Task Clamps_page_below_one_to_one()
    {
        var criteria = await HandleAsync(new SearchListingsQuery { Page = 0 });
        Assert.Equal(1, criteria.Page);
    }

    [Theory]
    [InlineData(0, 20)]   // unset → default
    [InlineData(5, 5)]    // within range → kept
    [InlineData(100, 50)] // above max → clamped
    public async Task Clamps_pageSize_to_bounds(int requested, int expected)
    {
        var criteria = await HandleAsync(new SearchListingsQuery { PageSize = requested });
        Assert.Equal(expected, criteria.PageSize);
    }

    [Fact]
    public async Task Invalid_sort_defaults_to_recent()
    {
        var criteria = await HandleAsync(new SearchListingsQuery { Sort = "bogus" });
        Assert.Equal("recent", criteria.Sort);
    }

    [Fact]
    public async Task Relevance_without_query_falls_back_to_recent()
    {
        var criteria = await HandleAsync(new SearchListingsQuery { Sort = "relevance" });
        Assert.Equal("recent", criteria.Sort);
    }

    [Fact]
    public async Task Relevance_with_query_is_kept_and_query_is_trimmed()
    {
        var criteria = await HandleAsync(new SearchListingsQuery { Sort = "relevance", Q = "  excavator  " });
        Assert.Equal("relevance", criteria.Sort);
        Assert.Equal("excavator", criteria.Query);
    }

    [Fact]
    public async Task Blank_filters_are_normalized_to_null()
    {
        var criteria = await HandleAsync(new SearchListingsQuery { Q = "  ", Category = "", Province = "   " });
        Assert.Null(criteria.Query);
        Assert.Null(criteria.CategorySlug);
        Assert.Null(criteria.Province);
    }
}
