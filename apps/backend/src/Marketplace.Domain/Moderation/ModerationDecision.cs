namespace Marketplace.Domain.Moderation;

/// <summary>Immutable audit record of a moderation action on a listing.</summary>
public class ModerationDecision : Entity
{
    public Guid ListingId { get; private set; }

    public Guid AdminId { get; private set; }

    public ModerationAction Decision { get; private set; }

    public string Reason { get; private set; }

    public DateTime CreatedAt { get; private set; }
}
