using Mediator;

namespace Marketplace.Domain.Common;

/// <summary>
/// Marker for domain events raised by aggregates and dispatched after the unit of work commits.
/// Extends <see cref="INotification"/> so events can be published via the mediator without any
/// wrapping or adapter — handlers implement <c>INotificationHandler&lt;TEvent&gt;</c> directly.
/// </summary>
public interface IDomainEvent : INotification;
