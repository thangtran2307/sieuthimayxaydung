using Marketplace.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Infrastructure;

/// <summary>
/// Registers the EF Core DbContext (write side). Other infrastructure services are registered via
/// the Autofac <c>InfrastructureModule</c>.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        NpgsqlBootstrap.Configure();

        // Write connection (EF Core write side + migrations).
        string writeConnectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("ConnectionStrings:Postgres is required.");

        services.AddDbContext<MarketplaceDbContext>(options =>
            options.UseNpgsql(writeConnectionString).UseSnakeCaseNamingConvention());

        // Read connection (Dapper read side) — can point at a read replica; falls back to write.
        string readConnectionString = configuration.GetConnectionString("PostgresRead") ?? writeConnectionString;
        services.AddSingleton(new ReadDbConnectionFactory(readConnectionString));

        return services;
    }
}
