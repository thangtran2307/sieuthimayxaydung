namespace Marketplace.Application.Categories;

/// <summary>Wire shape for a taxonomy node (category or subcategory).</summary>
public sealed record CategoryDto(
    Guid Id,
    string Slug,
    Guid? ParentId,
    string LabelVi,
    string LabelEn,
    string Icon,
    int SortOrder);
