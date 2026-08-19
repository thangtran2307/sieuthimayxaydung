namespace Marketplace.Domain.Common;

/// <summary>
/// Base for all persistence-tracked types: holds the domain-event buffer so any entity or
/// aggregate root can raise events that are dispatched after the unit of work commits.
/// Does NOT define an <c>Id</c> property — subclasses choose their own key shape.
/// Most aggregates should inherit <see cref="AggregateRoot"/> instead.
/// </summary>
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
