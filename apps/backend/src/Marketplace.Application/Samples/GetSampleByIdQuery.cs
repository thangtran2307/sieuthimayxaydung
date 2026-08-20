using Mediator;

namespace Marketplace.Application.Samples;

public sealed record GetSampleByIdQuery : IQuery<SampleDto>
{
    public Guid Id { get; init; }
}

public sealed class GetSampleByIdQueryHandler(ISampleQueries sampleQueries) : IQueryHandler<GetSampleByIdQuery, SampleDto>
{
    public async ValueTask<SampleDto> Handle(GetSampleByIdQuery query, CancellationToken cancellationToken)
    {
        return await sampleQueries.GetSampleByIdAsync(query.Id, cancellationToken);
    }
}
