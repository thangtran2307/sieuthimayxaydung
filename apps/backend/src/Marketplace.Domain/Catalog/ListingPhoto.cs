namespace Marketplace.Domain.Catalog;

/// <summary>A photo attached to a listing (gallery order via SortOrder; 0 = primary).</summary>
public class ListingPhoto : Entity
{
    public Guid ListingId { get; private set; }

    public string Url { get; private set; }

    public int SortOrder { get; private set; }

    public int? Width { get; private set; }

    public int? Height { get; private set; }
}
