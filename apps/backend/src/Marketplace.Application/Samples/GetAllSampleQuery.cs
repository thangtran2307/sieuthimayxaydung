using Mediator;

namespace Marketplace.Application.Samples;

public sealed record GetAllSampleQuery : IQuery<IReadOnlyList<SampleDto>>;

public sealed class GetAllSampleQueryHandler(ISampleQueries sampleQueries) : IQueryHandler<GetAllSampleQuery, IReadOnlyList<SampleDto>>
{
    public async ValueTask<IReadOnlyList<SampleDto>> Handle(GetAllSampleQuery query, CancellationToken cancellationToken)
    {
        return await sampleQueries.GetAllSamplesAsync(cancellationToken);
    }
}
