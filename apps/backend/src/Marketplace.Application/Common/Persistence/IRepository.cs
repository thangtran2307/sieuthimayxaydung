namespace Marketplace.Application.Common.Persistence;

/// <summary>
/// Generic write-side repository (Repository pattern) over aggregate roots. Persisting changes is
/// the job of <see cref="IUnitOfWork"/>. Read/query paths use Dapper (CQRS read/write split).
/// </summary>
public interface IRepository<T>
    where T : Entity
{
    Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    void Update(T entity);

    void Remove(T entity);
}
