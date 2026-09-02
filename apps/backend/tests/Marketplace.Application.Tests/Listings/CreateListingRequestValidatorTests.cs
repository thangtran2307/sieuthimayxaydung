using Marketplace.Application.Listings;
using Marketplace.Domain.Common;

namespace Marketplace.Application.Tests.Listings;

public sealed class CreateListingRequestValidatorTests
{
    private readonly CreateListingRequestValidator _validator = new();

    private static CreateListingRequest Valid() => new(
        Guid.NewGuid(),
        null,
        "Komatsu PC200",
        Condition.USED,
        850_000_000,
        false,
        "Hà Nội",
        "Well maintained.",
        null,
        [new ListingPhotoInput("https://cdn/1.jpg", 0, null, null)]);

    [Fact]
    public void Accepts_a_complete_request()
    {
        Assert.True(_validator.Validate(Valid()).IsValid);
    }

    [Fact]
    public void Requires_at_least_one_photo()
    {
        var request = Valid() with { Photos = [] };

        Assert.False(_validator.Validate(request).IsValid);
    }

    [Fact]
    public void Requires_a_price_unless_contact_for_price()
    {
        var request = Valid() with { PriceAmount = null, PriceContact = false };

        Assert.False(_validator.Validate(request).IsValid);
    }

    [Fact]
    public void Allows_a_missing_price_when_contact_for_price()
    {
        var request = Valid() with { PriceAmount = null, PriceContact = true };

        Assert.True(_validator.Validate(request).IsValid);
    }
}
