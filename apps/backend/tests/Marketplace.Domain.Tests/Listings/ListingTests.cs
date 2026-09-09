using Marketplace.Domain.Common;
using Marketplace.Domain.Listings;

namespace Marketplace.Domain.Tests.Listings;

public sealed class ListingTests
{
    private static readonly DateTime _now = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private static Listing NewListing(
        long? priceAmount = 100_000_000,
        bool priceContact = false,
        Guid sellerId = default) =>
        Listing.Create(
            sellerId == Guid.Empty ? Guid.NewGuid() : sellerId,
            "komatsu-pc200-abc123",
            new ListingDetails(
                Guid.NewGuid(),
                null,
                "Komatsu PC200",
                Condition.USED,
                priceAmount,
                priceContact,
                "Hà Nội",
                "Great condition.",
                new ListingSpecs(year: 2018)),
            _now);

    [Fact]
    public void RegisterView_increments_the_view_count()
    {
        var listing = new Listing();

        listing.RegisterView();
        listing.RegisterView();

        Assert.Equal(2, listing.ViewCount);
    }

    [Fact]
    public void Create_starts_pending_and_not_public()
    {
        var listing = NewListing();

        Assert.NotEqual(Guid.Empty, listing.Id);
        Assert.Equal(ListingStatus.PENDING, listing.Status);
        Assert.Equal("VND", listing.Currency);
        Assert.Equal(2018, listing.Specs.Year);
        Assert.Equal(_now, listing.CreatedAt);
    }

    [Fact]
    public void Create_with_contact_for_price_clears_the_amount()
    {
        var listing = NewListing(priceAmount: 999, priceContact: true);

        Assert.True(listing.PriceContact);
        Assert.Null(listing.PriceAmount);
    }

    [Fact]
    public void AddPhoto_attaches_photos_to_the_gallery()
    {
        var listing = NewListing();

        listing.AddPhoto("https://cdn/1.jpg", 0, 800, 600);
        listing.AddPhoto("https://cdn/2.jpg", 1);

        Assert.Equal(2, listing.Photos.Count);
        var first = Assert.Single(listing.Photos, p => p.SortOrder == 0);
        Assert.Equal(listing.Id, first.ListingId);
        Assert.Equal("https://cdn/1.jpg", first.Url);
    }

    [Fact]
    public void UpdateDetails_applies_edits_and_bumps_updated_at()
    {
        var listing = NewListing();
        var later = _now.AddHours(1);

        listing.UpdateDetails(
            new ListingDetails(
                listing.CategoryId,
                null,
                "Updated title",
                Condition.NEW,
                null,
                PriceContact: true,
                "Hồ Chí Minh",
                "New description",
                new ListingSpecs(brand: "Komatsu")),
            later);

        Assert.Equal("Updated title", listing.Title);
        Assert.Equal(Condition.NEW, listing.Condition);
        Assert.True(listing.PriceContact);
        Assert.Null(listing.PriceAmount);
        Assert.Equal("Hồ Chí Minh", listing.LocationProvince);
        Assert.Equal(later, listing.UpdatedAt);
    }

    [Fact]
    public void MarkSold_moves_a_listing_to_sold()
    {
        var listing = NewListing();

        listing.MarkSold(_now);

        Assert.Equal(ListingStatus.SOLD, listing.Status);
    }

    [Fact]
    public void MarkSold_throws_when_already_sold()
    {
        var listing = NewListing();
        listing.MarkSold(_now);

        Assert.Throws<DomainRuleException>(() => listing.MarkSold(_now));
    }

    [Fact]
    public void Remove_soft_deletes_the_listing()
    {
        var listing = NewListing();

        listing.Remove(_now);

        Assert.Equal(ListingStatus.REMOVED, listing.Status);
    }

    [Fact]
    public void Approve_publishes_a_pending_listing()
    {
        var listing = NewListing();
        var now = _now.AddHours(2);

        listing.Approve(now);

        Assert.Equal(ListingStatus.ACTIVE, listing.Status);
        Assert.Equal(now, listing.PublishedAt);
        Assert.Equal(now, listing.UpdatedAt);
    }

    [Fact]
    public void Approve_throws_when_not_pending()
    {
        var listing = NewListing();
        listing.Approve(_now);

        Assert.Throws<DomainRuleException>(() => listing.Approve(_now));
    }

    [Fact]
    public void Reject_marks_a_pending_listing_rejected()
    {
        var listing = NewListing();

        listing.Reject(_now);

        Assert.Equal(ListingStatus.REJECTED, listing.Status);
        Assert.Null(listing.PublishedAt);
    }

    [Fact]
    public void Reject_throws_when_not_pending()
    {
        var listing = NewListing();
        listing.Approve(_now);

        Assert.Throws<DomainRuleException>(() => listing.Reject(_now));
    }
}
