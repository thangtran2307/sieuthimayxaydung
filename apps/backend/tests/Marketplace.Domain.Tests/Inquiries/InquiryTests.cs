using Marketplace.Domain.Common;
using Marketplace.Domain.Inquiries;

namespace Marketplace.Domain.Tests.Inquiries;

public sealed class InquiryTests
{
    private static readonly DateTime Now = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void PhoneReveal_sets_type_ids_and_timestamp_only()
    {
        var listingId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();

        var inquiry = Inquiry.PhoneReveal(listingId, sellerId, Now);

        Assert.NotEqual(Guid.Empty, inquiry.Id);
        Assert.Equal(InquiryType.PHONE_REVEAL, inquiry.Type);
        Assert.Equal(listingId, inquiry.ListingId);
        Assert.Equal(sellerId, inquiry.SellerId);
        Assert.Equal(Now, inquiry.CreatedAt);
        Assert.Null(inquiry.BuyerName);
        Assert.Null(inquiry.Message);
    }

    [Fact]
    public void SendMessage_captures_buyer_details()
    {
        var inquiry = Inquiry.SendMessage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Buyer",
            "Is this still available?",
            Now,
            "0900000000",
            "buyer@example.com");

        Assert.Equal(InquiryType.MESSAGE, inquiry.Type);
        Assert.Equal("Buyer", inquiry.BuyerName);
        Assert.Equal("Is this still available?", inquiry.Message);
        Assert.Equal("0900000000", inquiry.BuyerPhone);
        Assert.Equal("buyer@example.com", inquiry.BuyerEmail);
        Assert.Equal(Now, inquiry.CreatedAt);
    }
}
