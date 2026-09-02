using Marketplace.Domain.Listings;

namespace Marketplace.Application.Listings;

/// <summary>Maps seller listing requests to the domain's <see cref="ListingDetails"/> carrier.</summary>
internal static class ListingMapping
{
    public static ListingDetails ToDetails(this CreateListingRequest request) => new(
        request.CategoryId,
        request.SubcategoryId,
        request.Title.Trim(),
        request.Condition,
        request.PriceAmount,
        request.PriceContact,
        request.LocationProvince.Trim(),
        request.Description.Trim(),
        ToSpecs(request.Specs));

    public static ListingDetails ToDetails(this UpdateListingRequest request) => new(
        request.CategoryId,
        request.SubcategoryId,
        request.Title.Trim(),
        request.Condition,
        request.PriceAmount,
        request.PriceContact,
        request.LocationProvince.Trim(),
        request.Description.Trim(),
        ToSpecs(request.Specs));

    private static ListingSpecs ToSpecs(ListingSpecsInput specs) =>
        specs is null
            ? new ListingSpecs()
            : new ListingSpecs(specs.Year, specs.Brand, specs.Model, specs.Hours, specs.Origin, specs.Capacity);
}
