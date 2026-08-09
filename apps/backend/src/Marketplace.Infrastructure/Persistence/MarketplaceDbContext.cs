using Marketplace.Domain.Catalog;
using Marketplace.Domain.Identity;
using Marketplace.Domain.Moderation;
using Marketplace.Domain.Promotion;
using InquiryEntity = Marketplace.Domain.Inquiry.Inquiry;

namespace Marketplace.Infrastructure.Persistence;

/// <summary>
/// EF Core context — the write side (commands) and the source of truth for auto-generated
/// migrations. Reads go through Dapper (CQRS split). Entity mappings live in Configurations/;
/// snake_case naming is applied at registration.
/// </summary>
public sealed class MarketplaceDbContext(DbContextOptions<MarketplaceDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Listing> Listings => Set<Listing>();

    public DbSet<ListingPhoto> ListingPhotos => Set<ListingPhoto>();

    public DbSet<BoostPackage> BoostPackages => Set<BoostPackage>();

    public DbSet<Boost> Boosts => Set<Boost>();

    public DbSet<Report> Reports => Set<Report>();

    public DbSet<ModerationDecision> ModerationDecisions => Set<ModerationDecision>();

    public DbSet<InquiryEntity> Inquiries => Set<InquiryEntity>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Store all timestamps as `timestamp without time zone`; values are always UTC.
        configurationBuilder.Properties<DateTime>().HaveColumnType("timestamp without time zone");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("pg_trgm");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MarketplaceDbContext).Assembly);
    }
}
