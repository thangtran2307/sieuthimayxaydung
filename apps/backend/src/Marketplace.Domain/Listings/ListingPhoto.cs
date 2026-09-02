namespace Marketplace.Domain.Listings;

/// <summary>A photo attached to a listing (gallery order via SortOrder; 0 = primary).</summary>
public class ListingPhoto : Entity<Guid>
{
    public Guid ListingId { get; private set; }

    public string Url { get; private set; }

    public int SortOrder { get; private set; }

    public int? Width { get; private set; }

    public int? Height { get; private set; }

    /// <summary>Creates a photo for a listing's gallery.</summary>
    public static ListingPhoto Create(Guid listingId, string url, int sortOrder, int? width = null, int? height = null) => new()
    {
        Id = Guid.NewGuid(),
        ListingId = listingId,
        Url = url,
        SortOrder = sortOrder,
        Width = width,
        Height = height,
    };
}
