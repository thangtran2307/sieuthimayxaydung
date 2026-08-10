using Marketplace.Application.Common.Persistence;

namespace Marketplace.Api.Startup;

/// <summary>
/// One-shot CLI verbs (<c>migrate</c>, <c>seed</c>) that run against the built host and exit.
/// Returns true when a verb was handled so the caller can stop before starting the web server.
/// </summary>
internal static class MaintenanceCli
{
    public static async Task<bool> TryRunAsync(WebApplication app, string[] args)
    {
        if (args.Length == 0 || args[0] is not ("migrate" or "seed"))
        {
            return false;
        }

        using var scope = app.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<IDatabaseMigrator>().Migrate();
        if (args[0] == "seed")
        {
            await scope.ServiceProvider.GetRequiredService<IDataSeeder>().SeedAsync();
        }

        return true;
    }
}
