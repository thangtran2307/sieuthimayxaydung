namespace Marketplace.Domain.Boosts;

/// <summary>A boost applied (or requested) for one listing; lifecycle REQUESTED→ACTIVE→EXPIRED.</summary>
public class Boost : Entity<Guid>, IAggregateRoot
{
    public Guid ListingId { get; private set; }

    public Guid SellerId { get; private set; }

    public Guid PackageId { get; private set; }

    public int PriorityLevel { get; private set; }

    public BoostStatus Status { get; private set; } = BoostStatus.REQUESTED;

    public string PaymentReference { get; private set; }

    public DateTime RequestedAt { get; private set; }

    public DateTime? ActivatedAt { get; private set; }

    public Guid? ActivatedByAdminId { get; private set; }

    public DateTime? StartsAt { get; private set; }

    public DateTime? ExpiresAt { get; private set; }
}
