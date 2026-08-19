using Marketplace.Domain.Categories;
using Marketplace.Domain.Identities;
using Marketplace.Domain.Listings;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Infrastructure.Persistence.Configurations;

internal sealed class ListingConfiguration : IEntityTypeConfiguration<Listing>
{
    public void Configure(EntityTypeBuilder<Listing> builder)
    {
        builder.ToTable("listings");
        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.SellerId).HasColumnName("seller_id");
        builder.Property(x => x.CategoryId).HasColumnName("category_id");
        builder.Property(x => x.SubcategoryId).HasColumnName("subcategory_id");
        builder.Property(x => x.Title).HasColumnName("title");
        builder.Property(x => x.Slug).HasColumnName("slug");
        builder.Property(x => x.Condition).HasColumnName("condition").HasConversion<string>().HasMaxLength(16);
        builder.Property(x => x.PriceAmount).HasColumnName("price_amount");
        builder.Property(x => x.PriceContact).HasColumnName("price_contact").HasDefaultValue(false);
        builder.Property(x => x.Currency).HasColumnName("currency").HasMaxLength(8).HasDefaultValue("VND");
        builder.Property(x => x.LocationProvince).HasColumnName("location_province");
        builder.Property(x => x.Description).HasColumnName("description");
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(16);
        builder.Property(x => x.ViewCount).HasColumnName("view_count").HasDefaultValue(0);
        builder.Property(x => x.SearchVector).HasColumnName("search_vector");
        builder.Property(x => x.PublishedAt).HasColumnName("published_at");
        builder.Property(x => x.ExpiresAt).HasColumnName("expires_at");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("timezone('utc', now())");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("timezone('utc', now())");

        // Specifications value object → jsonb column.
        builder.OwnsOne(x => x.Specs, owned => owned.ToJson("specs"));

        // Full-text search: Postgres-generated tsvector column + GIN index.
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
