using Marketplace.Application.Common.Models;
using Mediator;

namespace Marketplace.Application.Catalog;

/// <summary>
/// Search / filter / sort public listings. Bound directly from the query string on the API. All
/// pagination values are CLAMPED (never rejected) per Constitution II &amp; VII.
/// </summary>
public sealed record SearchListingsQuery : IQuery<PagedResult<ListingSummaryDto>>
{
    public string Q { get; init; }

    public string Category { get; init; }

    public string Subcategory { get; init; }

    public Condition? Condition { get; init; }

    public string Province { get; init; }

    public long? PriceMin { get; init; }

    public long? PriceMax { get; init; }

    public string Sort { get; init; }

    public int Page { get; init; }

    public int PageSize { get; init; }
}

public sealed class SearchListingsQueryHandler(IListingQueries listingQueries)
    : IQueryHandler<SearchListingsQuery, PagedResult<ListingSummaryDto>>
{
    private const int _defaultPageSize = 20;
    private const int _maxPageSize = 50;
    private static readonly string[] _allowedSorts = ["recent", "price_asc", "price_desc", "relevance"];

    public async ValueTask<PagedResult<ListingSummaryDto>> Handle(
        SearchListingsQuery query,
        CancellationToken cancellationToken)
    {
        int page = query.Page < 1 ? 1 : query.Page;
        int pageSize = query.PageSize <= 0 ? _defaultPageSize : Math.Min(query.PageSize, _maxPageSize);
        string sort = _allowedSorts.Contains(query.Sort) ? query.Sort : "recent";

        // Relevance only makes sense with a text query; otherwise fall back to most recent.
        if (sort == "relevance" && string.IsNullOrWhiteSpace(query.Q))
        {
            sort = "recent";
        }

        var criteria = new ListingSearchCriteria(
            string.IsNullOrWhiteSpace(query.Q) ? null : query.Q.Trim(),
            string.IsNullOrWhiteSpace(query.Category) ? null : query.Category,
            string.IsNullOrWhiteSpace(query.Subcategory) ? null : query.Subcategory,
            query.Condition,
            string.IsNullOrWhiteSpace(query.Province) ? null : query.Province,
            query.PriceMin,
            query.PriceMax,
            sort,
            page,
            pageSize);

        return await listingQueries.SearchAsync(criteria, cancellationToken);
    }
}
