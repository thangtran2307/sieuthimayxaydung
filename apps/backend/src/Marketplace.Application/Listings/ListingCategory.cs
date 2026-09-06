using Marketplace.Application.Categories;

namespace Marketplace.Application.Listings;

/// <summary>
/// Validates the category/subcategory referenced by a listing before it is persisted, so a bad id is
/// reported as a clear 422 at the boundary instead of surfacing later as a foreign-key 500.
/// </summary>
internal static class ListingCategory
{
    public static async Task EnsureValidAsync(
        ICategoryQueries categoryQueries,
        Guid categoryId,
        Guid? subcategoryId,
        CancellationToken cancellationToken)
    {
        var category = await categoryQueries.GetByIdAsync(categoryId, cancellationToken);
        if (category is null || category.ParentId is not null)
        {
            throw new DomainRuleException("Category not found.", ErrorCodes.CategoryNotFound);
        }

        if (subcategoryId is { } subId && subId != Guid.Empty)
        {
            var subcategory = await categoryQueries.GetByIdAsync(subId, cancellationToken);
            if (subcategory is null || subcategory.ParentId != categoryId)
            {
                throw new DomainRuleException(
                    "Subcategory does not belong to the selected category.",
                    ErrorCodes.InvalidSubcategory);
            }
        }
    }
}
