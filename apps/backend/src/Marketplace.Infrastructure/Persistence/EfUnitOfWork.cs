using Marketplace.Application.Common.Persistence;

namespace Marketplace.Infrastructure.Persistence;

/// <summary>EF Core unit of work — commits all tracked changes in one transaction.</summary>
public sealed class EfUnitOfWork(MarketplaceDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        dbContext.SaveChangesAsync(cancellationToken);
}
