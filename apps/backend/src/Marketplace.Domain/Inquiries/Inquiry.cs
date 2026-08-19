namespace Marketplace.Domain.Inquiries;

/// <summary>A buyer's contact action against a listing (phone reveal or message).</summary>
public class Inquiry : Entity<Guid>, IAggregateRoot
{
    public Guid ListingId { get; private set; }

    public Guid SellerId { get; private set; }

    public InquiryType Type { get; private set; }

    public string BuyerName { get; private set; }

    public string BuyerPhone { get; private set; }

    public string BuyerEmail { get; private set; }

    public string Message { get; private set; }

    public DateTime CreatedAt { get; private set; }

    /// <summary>Records that a buyer revealed the seller's phone number (no message).</summary>
    public static Inquiry PhoneReveal(Guid listingId, Guid sellerId, DateTime now) => new()
    {
        Id = Guid.NewGuid(),
        ListingId = listingId,
        SellerId = sellerId,
        Type = InquiryType.PHONE_REVEAL,
        CreatedAt = now,
    };

    /// <summary>Records a buyer's message to the seller. Name and message are required.</summary>
    public static Inquiry SendMessage(
        Guid listingId,
        Guid sellerId,
        string buyerName,
        string message,
        DateTime now,
        string buyerPhone = null,
        string buyerEmail = null) => new()
        {
            Id = Guid.NewGuid(),
            ListingId = listingId,
            SellerId = sellerId,
            Type = InquiryType.MESSAGE,
            BuyerName = buyerName,
            Message = message,
            BuyerPhone = buyerPhone,
            BuyerEmail = buyerEmail,
            CreatedAt = now,
        };
}
