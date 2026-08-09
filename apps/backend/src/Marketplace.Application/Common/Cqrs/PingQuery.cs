using Mediator;

namespace Marketplace.Application.Common.Cqrs;

/// <summary>
/// Trivial CQRS query proving the source-generated mediator + DI wiring end-to-end.
/// Real queries/commands per bounded context are added in the user-story phases.
/// </summary>
public sealed record PingQuery(string Message) : IQuery<PingResult>;

public sealed record PingResult(string Reply);

public sealed class PingQueryHandler : IQueryHandler<PingQuery, PingResult>
{
    public ValueTask<PingResult> Handle(PingQuery query, CancellationToken cancellationToken) =>
        ValueTask.FromResult(new PingResult($"pong: {query.Message}"));
}
