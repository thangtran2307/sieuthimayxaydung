using Marketplace.Domain.Listings;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Infrastructure.Persistence.Configurations;

internal sealed class ListingPhotoConfiguration : IEntityTypeConfiguration<ListingPhoto>
{
    public void Configure(EntityTypeBuilder<ListingPhoto> builder)
    {
        builder.ToTable("listing_photos");
        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.ListingId).HasColumnName("listing_id");
        builder.Property(x => x.Url).HasColumnName("url");
        builder.Property(x => x.SortOrder).HasColumnName("sort_order");
        builder.Property(x => x.Width).HasColumnName("width");
        builder.Property(x => x.Height).HasColumnName("height");

        builder.HasIndex(x => x.ListingId);
    }
}
