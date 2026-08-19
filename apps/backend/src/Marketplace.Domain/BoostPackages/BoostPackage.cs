namespace Marketplace.Domain.BoostPackages;

/// <summary>A purchasable boost tier (Basic / Featured / Max).</summary>
public class BoostPackage : Entity<Guid>, IAggregateRoot
{
    public BoostTier Tier { get; private set; }

    public string Name { get; private set; }

    public int PriorityLevel { get; private set; }

    public int DurationDays { get; private set; }

    public long PriceAmount { get; private set; }

    public bool Active { get; private set; } = true;
}
