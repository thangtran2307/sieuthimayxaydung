using Marketplace.Application.Common.Persistence;

namespace Marketplace.Infrastructure.Persistence;

/// <summary>Generic EF Core write repository (Repository pattern). Commit via <see cref="EfUnitOfWork"/>.</summary>
public sealed class EfRepository<T>(MarketplaceDbContext dbContext) : IRepository<T>
    where T : Entity
{
    public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await dbContext.Set<T>().FindAsync([id], cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
        await dbContext.Set<T>().AddAsync(entity, cancellationToken);

    public void Update(T entity) => dbContext.Set<T>().Update(entity);

    public void Remove(T entity) => dbContext.Set<T>().Remove(entity);
}
