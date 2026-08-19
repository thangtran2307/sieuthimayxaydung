using Marketplace.Domain.BoostPackages;
using Marketplace.Domain.Boosts;
using Marketplace.Domain.Identities;
using Marketplace.Domain.Listings;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Infrastructure.Persistence.Configurations;

internal sealed class BoostConfiguration : IEntityTypeConfiguration<Boost>
{
    public void Configure(EntityTypeBuilder<Boost> builder)
    {
        builder.ToTable("boosts");
        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.ListingId).HasColumnName("listing_id");
        builder.Property(x => x.SellerId).HasColumnName("seller_id");
        builder.Property(x => x.PackageId).HasColumnName("package_id");
        builder.Property(x => x.PriorityLevel).HasColumnName("priority_level");
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(16);
        builder.Property(x => x.PaymentReference).HasColumnName("payment_reference");
        builder.Property(x => x.RequestedAt).HasColumnName("requested_at").HasDefaultValueSql("timezone('utc', now())");
        builder.Property(x => x.ActivatedAt).HasColumnName("activated_at");
        builder.Property(x => x.ActivatedByAdminId).HasColumnName("activated_by_admin_id");
        builder.Property(x => x.StartsAt).HasColumnName("starts_at");
        builder.Property(x => x.ExpiresAt).HasColumnName("expires_at");

        builder.HasIndex(x => x.ListingId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.ExpiresAt);

        builder.HasOne<Listing>().WithMany().HasForeignKey(x => x.ListingId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.SellerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<BoostPackage>().WithMany().HasForeignKey(x => x.PackageId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ActivatedByAdminId).OnDelete(DeleteBehavior.Restrict);
    }
}
