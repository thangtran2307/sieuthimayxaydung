namespace Marketplace.Domain.Common;

/// <summary>
/// Base class for all entities/aggregate roots: identity plus a domain-event buffer so behavior
/// added in later phases can raise events that are dispatched after the unit of work commits.
/// </summary>
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public Guid Id { get; protected set; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
