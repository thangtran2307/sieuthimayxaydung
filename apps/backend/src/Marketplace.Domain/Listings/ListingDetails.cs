namespace Marketplace.Domain.Listings;

/// <summary>
/// The mutable, seller-supplied fields of a listing — shared by <see cref="Listing.Create"/> and
/// <see cref="Listing.UpdateDetails"/> so neither carries a long parameter list.
/// </summary>
public sealed record ListingDetails(
    Guid CategoryId,
    Guid? SubcategoryId,
    string Title,
    Condition Condition,
    long? PriceAmount,
    bool PriceContact,
    string LocationProvince,
    string Description,
    ListingSpecs Specs);
