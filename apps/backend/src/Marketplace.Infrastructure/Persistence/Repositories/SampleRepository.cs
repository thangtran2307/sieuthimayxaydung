using Marketplace.Domain.Samples;

namespace Marketplace.Infrastructure.Persistence.Repositories;

internal sealed class SampleRepository(MarketplaceDbContext dbContext) : BaseEfRepository<Sample>(dbContext), ISampleRepository
{
    public async Task<Sample> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<Sample>().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
}
