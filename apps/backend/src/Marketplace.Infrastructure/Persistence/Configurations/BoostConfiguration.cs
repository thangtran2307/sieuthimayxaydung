using Marketplace.Domain.Catalog;
using Marketplace.Domain.Identity;
using Marketplace.Domain.Promotion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Infrastructure.Persistence.Configurations;

internal sealed class BoostConfiguration : IEntityTypeConfiguration<Boost>
{
    public void Configure(EntityTypeBuilder<Boost> builder)
    {
        builder.Ignore(x => x.DomainEvents);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(16);
        builder.Property(x => x.RequestedAt).HasDefaultValueSql("timezone('utc', now())");
        builder.HasIndex(x => x.ListingId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.ExpiresAt);
        builder.HasOne<Listing>().WithMany().HasForeignKey(x => x.ListingId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.SellerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<BoostPackage>().WithMany().HasForeignKey(x => x.PackageId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ActivatedByAdminId).OnDelete(DeleteBehavior.Restrict);
    }
}
