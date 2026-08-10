using Asp.Versioning;
using Marketplace.Api.Security;
using Marketplace.Application.Common.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Controllers;

/// <summary>
/// Internal maintenance endpoints, protected by an API key (<c>X-Api-Key</c>). Reusable host for
/// future internal/operational tasks. Not part of the public API surface.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("internal")]
[ApiKeyAuth]
public sealed class InternalController(IDatabaseMigrator databaseMigrator, IDataSeeder dataSeeder)
    : ControllerBase
{
    /// <summary>Applies pending migrations then seeds reference + admin data (idempotent).</summary>
    [HttpPost("seed")]
    public async Task<IActionResult> Seed(CancellationToken cancellationToken)
    {
        databaseMigrator.Migrate();
        await dataSeeder.SeedAsync(cancellationToken);
        return Ok(new { seeded = true });
    }
}
