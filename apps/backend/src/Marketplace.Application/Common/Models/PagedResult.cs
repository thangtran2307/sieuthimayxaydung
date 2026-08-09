using System.Collections;

namespace Marketplace.Application.Common.Models;

/// <summary>Pagination metadata mirroring the wire contract's `meta` object.</summary>
public sealed record PageMeta(int Page, int PageSize, int Total, int TotalPages);

/// <summary>Non-generic view of a paged result so the API envelope filter can emit `{ data, meta }`.</summary>
public interface IPagedResult
{
    IEnumerable Items { get; }

    PageMeta Meta { get; }
}

/// <summary>A page of results plus its metadata, returned by query handlers.</summary>
public sealed record PagedResult<T>(IReadOnlyList<T> Items, PageMeta Meta) : IPagedResult
{
    IEnumerable IPagedResult.Items => Items;

    public static PagedResult<T> Create(IReadOnlyList<T> items, int total, int page, int pageSize)
    {
        var totalPages = pageSize > 0 ? (int)Math.Ceiling(total / (double)pageSize) : 0;
        return new PagedResult<T>(items, new PageMeta(page, pageSize, total, totalPages));
    }
}
