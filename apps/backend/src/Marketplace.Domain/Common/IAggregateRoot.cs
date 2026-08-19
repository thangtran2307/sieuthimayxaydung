namespace Marketplace.Domain.Common;

/// <summary>
/// Marker for true aggregate roots — the single entry point to an aggregate that may own a
/// repository and raises domain events. Entities that exist only inside another aggregate (e.g.,
/// <c>ListingPhoto</c> inside the Listing aggregate) should NOT implement this interface.
/// </summary>
public interface IAggregateRoot;
