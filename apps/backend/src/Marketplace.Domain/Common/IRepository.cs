namespace Marketplace.Domain.Common;

/// <summary>
/// Write-side repository (Repository pattern) over an aggregate root — load, add, update, remove.
/// A repository contract is a domain concept, so it lives in the Domain layer. Persisting changes
/// is the job of the unit of work. Reads/queries use the Dapper query classes (CQRS read/write split).
/// </summary>
public interface IRepository<T>
    where T : Entity, IAggregateRoot
{
    Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Loads all aggregates matching the predicate in a single query (e.g. for bulk actions).</summary>
    Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    void Update(T entity);

    void Remove(T entity);
}
