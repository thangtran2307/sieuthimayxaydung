namespace Marketplace.Domain.ModerationDecisions;

/// <summary>Immutable audit record of a moderation action on a listing.</summary>
public class ModerationDecision : Entity<Guid>, IAggregateRoot
{
    public Guid ListingId { get; private set; }

    public Guid AdminId { get; private set; }

    public ModerationAction Decision { get; private set; }

    public string Reason { get; private set; }

    public DateTime CreatedAt { get; private set; }

    /// <summary>Records an admin's moderation action on a listing (immutable audit trail, FR-030).</summary>
    public static ModerationDecision Create(
        Guid listingId,
        Guid adminId,
        ModerationAction decision,
        DateTime now,
        string reason = null) => new()
        {
            Id = Guid.NewGuid(),
            ListingId = listingId,
            AdminId = adminId,
            Decision = decision,
            Reason = reason,
            CreatedAt = now,
        };
}
