namespace Marketplace.Application.Samples;

public interface ISampleQueries
{
    Task<SampleDto> GetSampleByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SampleDto>> GetAllSamplesAsync(CancellationToken cancellationToken = default);
}
