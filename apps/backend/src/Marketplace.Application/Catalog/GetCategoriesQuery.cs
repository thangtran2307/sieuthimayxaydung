using Mediator;

namespace Marketplace.Application.Catalog;

/// <summary>Returns the full two-level category taxonomy (categories + subcategories).</summary>
public sealed record GetCategoriesQuery : IQuery<IReadOnlyList<CategoryDto>>;

public sealed class GetCategoriesQueryHandler(ICategoryQueries categoryQueries)
    : IQueryHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    public async ValueTask<IReadOnlyList<CategoryDto>> Handle(
        GetCategoriesQuery query,
        CancellationToken cancellationToken) =>
        await categoryQueries.GetAllAsync(cancellationToken);
}
