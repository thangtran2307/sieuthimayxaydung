using Marketplace.Application.Common.Persistence;
using Marketplace.Infrastructure.Persistence;

namespace Marketplace.Infrastructure.Migrations;

/// <summary>Applies pending EF Core migrations to the database.</summary>
public sealed class EfCoreMigrator(MarketplaceDbContext dbContext) : IDatabaseMigrator
{
    public void Migrate() => dbContext.Database.Migrate();
}
