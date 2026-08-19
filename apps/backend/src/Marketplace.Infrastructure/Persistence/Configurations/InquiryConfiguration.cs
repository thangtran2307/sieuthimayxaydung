using Marketplace.Domain.Identities;
using Marketplace.Domain.Inquiries;
using Marketplace.Domain.Listings;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Infrastructure.Persistence.Configurations;

internal sealed class InquiryConfiguration : IEntityTypeConfiguration<Inquiry>
{
    public void Configure(EntityTypeBuilder<Inquiry> builder)
    {
        builder.ToTable("inquiries");
        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.ListingId).HasColumnName("listing_id");
        builder.Property(x => x.SellerId).HasColumnName("seller_id");
        builder.Property(x => x.Type).HasColumnName("type").HasConversion<string>().HasMaxLength(16);
        builder.Property(x => x.BuyerName).HasColumnName("buyer_name");
        builder.Property(x => x.BuyerPhone).HasColumnName("buyer_phone");
        builder.Property(x => x.BuyerEmail).HasColumnName("buyer_email");
        builder.Property(x => x.Message).HasColumnName("message");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("timezone('utc', now())");

        builder.HasIndex(x => x.ListingId);
        builder.HasIndex(x => x.SellerId);

        builder.HasOne<Listing>().WithMany().HasForeignKey(x => x.ListingId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.SellerId).OnDelete(DeleteBehavior.Restrict);
    }
}
