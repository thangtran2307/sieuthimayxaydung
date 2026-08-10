namespace Marketplace.Infrastructure.Persistence;

/// <summary>
/// Generic EF Core write repository (Repository pattern) over an aggregate root — load, add,
/// update, remove. Reached through <see cref="EfUnitOfWork"/>; changes are committed by the unit
/// of work. A dedicated per-aggregate repository is introduced only when it needs custom write
/// methods (avoiding empty interface indirection).
/// </summary>
internal sealed class EfRepository<T>(MarketplaceDbContext dbContext) : IRepository<T>
    where T : Entity
{
    public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Set<T>().FindAsync([id], cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await dbContext.Set<T>().AddAsync(entity, cancellationToken);

    public void Update(T entity) => dbContext.Set<T>().Update(entity);

    public void Remove(T entity) => dbContext.Set<T>().Remove(entity);
}
