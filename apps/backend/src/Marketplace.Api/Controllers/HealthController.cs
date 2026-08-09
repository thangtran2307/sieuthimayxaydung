using Marketplace.Application.Common.Cqrs;
using Marketplace.Infrastructure.Persistence;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Controllers;

/// <summary>
/// Liveness/readiness probe. Also dispatches a trivial CQRS query to prove the mediator + Autofac
/// wiring. Wrapped as `{ data: {...} }` by the envelope filter.
/// </summary>
[ApiController]
[Route("health")]
public sealed class HealthController(MarketplaceDbContext dbContext, IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<object> Get(CancellationToken cancellationToken)
    {
        var database = await dbContext.Database.CanConnectAsync(cancellationToken) ? "up" : "down";
        var ping = await mediator.Send(new PingQuery("health"), cancellationToken);

        return new
        {
            status = "ok",
            database,
            ping = ping.Reply,
            time = DateTime.UtcNow,
        };
    }
}
