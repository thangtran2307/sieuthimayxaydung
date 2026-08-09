namespace Marketplace.Application.Common.Persistence;

/// <summary>Seeds reference and demo data (categories, boost packages, admin, samples).</summary>
public interface IDataSeeder
{
    Task SeedAsync(CancellationToken cancellationToken = default);
}
