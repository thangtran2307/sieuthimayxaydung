namespace Marketplace.Application.Listings;

/// <summary>Technical specifications supplied on the listing form (all optional).</summary>
public sealed record ListingSpecsInput(
    int? Year,
    string Brand,
    string Model,
    int? Hours,
    string Origin,
    string Capacity);

/// <summary>A photo attached to a new listing (already uploaded; referenced by URL).</summary>
public sealed record ListingPhotoInput(string Url, int SortOrder, int? Width, int? Height);

/// <summary>Result of creating a listing — enough for the client to route to it.</summary>
public sealed record CreatedListingDto(Guid Id, string Slug, ListingStatus Status);

/// <summary>A row in the seller's own dashboard — includes non-public statuses (FR-018).</summary>
public sealed record SellerListingDto(
    Guid Id,
    string Slug,
    string Title,
    ListingStatus Status,
    long? PriceAmount,
    bool PriceContact,
    string Currency,
    int ViewCount,
    string ThumbnailUrl,
    DateTime CreatedAt,
    DateTime? PublishedAt);

/// <summary>Read-back of a listing's technical specifications for the edit form.</summary>
public sealed record ListingSpecsDto(
    int? Year,
    string Brand,
    string Model,
    int? Hours,
    string Origin,
    string Capacity);

/// <summary>
/// The current editable state of a seller's own listing, used to pre-fill the edit form. Photos are
/// not included — photo management is a separate concern from <see cref="UpdateListingCommand"/>.
/// </summary>
public sealed record SellerListingDetailDto(
    Guid Id,
    Guid CategoryId,
    Guid? SubcategoryId,
    string Title,
    Condition Condition,
    long? PriceAmount,
    bool PriceContact,
    string LocationProvince,
    string Description,
    ListingSpecsDto Specs,
    ListingStatus Status);
