namespace Marketplace.Domain.Samples;

public interface ISampleRepository : IRepository<Sample>
{
    Task<Sample> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
