namespace Marketplace.Application.Catalog;

/// <summary>
/// Normalized (clamped) criteria passed to the read side for a listing search. Only ACTIVE listings
/// are returned; boosted listings are always ranked first (see the Dapper repository).
/// </summary>
public sealed record ListingSearchCriteria(
    string Query,
    string CategorySlug,
    string SubcategorySlug,
    Condition? Condition,
    string Province,
    long? PriceMin,
    long? PriceMax,
    string Sort,
    int Page,
    int PageSize);

/// <summary>Contact details needed to record an inquiry against a listing.</summary>
public sealed record ListingContact(Guid ListingId, Guid SellerId, ListingStatus Status, string SellerPhone);
