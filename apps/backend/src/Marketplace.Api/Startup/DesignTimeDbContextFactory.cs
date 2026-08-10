using Marketplace.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Marketplace.Api.Startup;

/// <summary>
/// Builds <see cref="MarketplaceDbContext"/> for <c>dotnet ef</c> at design time WITHOUT booting the
/// full app (Autofac + options validation). It reads the SAME configuration as runtime — appsettings,
/// environment variables, and user-secrets — so migrations always target the configured database.
/// No connection is opened for <c>migrations add</c>; only <c>database update</c> connects.
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MarketplaceDbContext>
{
    public MarketplaceDbContext CreateDbContext(string[] args)
    {
        NpgsqlBootstrap.Configure();

        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddUserSecrets<DesignTimeDbContextFactory>(optional: true)
            .AddEnvironmentVariables()
            .Build();

        string connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException(
                "ConnectionStrings:Postgres is required for design-time migrations. Set it in " +
                "appsettings.json, user-secrets, or the environment (ConnectionStrings__Postgres).");

        DbContextOptions<MarketplaceDbContext> options = new DbContextOptionsBuilder<MarketplaceDbContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        return new MarketplaceDbContext(options);
    }
}
