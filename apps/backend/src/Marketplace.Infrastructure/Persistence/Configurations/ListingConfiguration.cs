using Marketplace.Domain.Catalog;
using Marketplace.Domain.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Infrastructure.Persistence.Configurations;

internal sealed class ListingConfiguration : IEntityTypeConfiguration<Listing>
{
    public void Configure(EntityTypeBuilder<Listing> builder)
    {
        builder.Ignore(x => x.DomainEvents);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.Condition).HasConversion<string>().HasMaxLength(16);
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(16);
        builder.Property(x => x.Currency).HasMaxLength(8).HasDefaultValue("VND");
        builder.Property(x => x.ViewCount).HasDefaultValue(0);
        builder.Property(x => x.PriceContact).HasDefaultValue(false);
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("timezone('utc', now())");
        builder.Property(x => x.UpdatedAt).HasDefaultValueSql("timezone('utc', now())");

        // Specifications value object → jsonb (EF Core owned entity mapped to JSON).
        builder.OwnsOne(x => x.Specs, owned => owned.ToJson());

        // Full-text search: Postgres-generated tsvector column + GIN index, per the Npgsql
        // full-text-search docs (https://www.npgsql.org/efcore/mapping/full-text-search.html).
        builder.HasGeneratedTsVectorColumn(x => x.SearchVector, "simple", x => new { x.Title, x.Description })
            .HasIndex(x => x.SearchVector)
            .HasMethod("GIN");

        // Fuzzy title matching (pg_trgm).
        builder.HasIndex(x => x.Title).HasMethod("gin").HasOperators("gin_trgm_ops");

        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CategoryId);
        builder.HasIndex(x => x.SubcategoryId);
        builder.HasIndex(x => x.LocationProvince);
        builder.HasIndex(x => x.PriceAmount);
        builder.HasIndex(x => x.PublishedAt);
        builder.HasIndex(x => x.SellerId);

        builder.HasOne<User>().WithMany().HasForeignKey(x => x.SellerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Category>().WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Category>().WithMany().HasForeignKey(x => x.SubcategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.Photos).WithOne().HasForeignKey(p => p.ListingId).OnDelete(DeleteBehavior.Cascade);
    }
}
