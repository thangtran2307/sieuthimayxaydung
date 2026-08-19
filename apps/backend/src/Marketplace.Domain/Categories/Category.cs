namespace Marketplace.Domain.Categories;

/// <summary>Two-level machinery taxonomy (category → subcategory) with bilingual labels.</summary>
public class Category : Entity<Guid>, IAggregateRoot
{
    public string Slug { get; private set; }

    public Guid? ParentId { get; private set; }

    public Category Parent { get; private set; }

    public ICollection<Category> Children { get; private set; } = new List<Category>();

    public string LabelVi { get; private set; }

    public string LabelEn { get; private set; }

    public string Icon { get; private set; }

    public int SortOrder { get; private set; }
}
