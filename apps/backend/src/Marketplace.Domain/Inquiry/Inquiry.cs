namespace Marketplace.Domain.Inquiry;

/// <summary>A buyer's contact action against a listing (phone reveal or message).</summary>
public class Inquiry : Entity
{
    public Guid ListingId { get; private set; }

    public Guid SellerId { get; private set; }

    public InquiryType Type { get; private set; }

    public string BuyerName { get; private set; }

    public string BuyerPhone { get; private set; }

    public string BuyerEmail { get; private set; }

    public string Message { get; private set; }

    public DateTime CreatedAt { get; private set; }
}
