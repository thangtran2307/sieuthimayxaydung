namespace Marketplace.Infrastructure.Persistence.Repositories;

/// <summary>
/// Shared EF Core implementation of <see cref="IRepository{T}"/>. Concrete per-aggregate
/// repositories inherit this to get the standard CRUD operations and can add custom methods.
/// </summary>
internal abstract class BaseEfRepository<T>(MarketplaceDbContext dbContext) : IRepository<T>
    where T : Entity, IAggregateRoot
{
    protected MarketplaceDbContext DbContext { get; } = dbContext;

    public async Task<T> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default) =>
        await DbContext.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await DbContext.Set<T>().AddAsync(entity, cancellationToken);

    public void Update(T entity) => DbContext.Set<T>().Update(entity);

    public void Remove(T entity) => DbContext.Set<T>().Remove(entity);
}
