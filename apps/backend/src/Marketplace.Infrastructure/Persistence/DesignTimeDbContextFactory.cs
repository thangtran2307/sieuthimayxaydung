using Microsoft.EntityFrameworkCore.Design;

namespace Marketplace.Infrastructure.Persistence;

/// <summary>
/// Lets `dotnet ef migrations` build the context at design time without running the app.
/// No database connection is opened for `migrations add` (only for `database update`).
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MarketplaceDbContext>
{
    public MarketplaceDbContext CreateDbContext(string[] args)
    {
        NpgsqlBootstrap.Configure();

        var connectionString =
            Environment.GetEnvironmentVariable("MARKETPLACE_DB")
            ?? "Host=localhost;Port=5432;Database=smxd;Username=smxd;Password=smxd";

        var options = new DbContextOptionsBuilder<MarketplaceDbContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;

        return new MarketplaceDbContext(options);
    }
}
