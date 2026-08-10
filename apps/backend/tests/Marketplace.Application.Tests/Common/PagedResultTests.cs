using Marketplace.Application.Common.Models;

namespace Marketplace.Application.Tests.Common;

public sealed class PagedResultTests
{
    [Theory]
    [InlineData(0, 20, 0)]
    [InlineData(20, 20, 1)]
    [InlineData(45, 20, 3)]
    [InlineData(1, 20, 1)]
    public void Create_computes_total_pages(int total, int pageSize, int expectedTotalPages)
    {
        var result = PagedResult<string>.Create([], total, 1, pageSize);

        Assert.Equal(total, result.Meta.Total);
        Assert.Equal(pageSize, result.Meta.PageSize);
        Assert.Equal(expectedTotalPages, result.Meta.TotalPages);
    }
}
