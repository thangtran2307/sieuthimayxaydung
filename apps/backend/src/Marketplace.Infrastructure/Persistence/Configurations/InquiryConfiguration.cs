using Marketplace.Domain.Catalog;
using Marketplace.Domain.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InquiryEntity = Marketplace.Domain.Inquiry.Inquiry;

namespace Marketplace.Infrastructure.Persistence.Configurations;

internal sealed class InquiryConfiguration : IEntityTypeConfiguration<InquiryEntity>
{
    public void Configure(EntityTypeBuilder<InquiryEntity> builder)
    {
        builder.Ignore(x => x.DomainEvents);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(16);
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("timezone('utc', now())");
        builder.HasIndex(x => x.ListingId);
        builder.HasIndex(x => x.SellerId);
        builder.HasOne<Listing>().WithMany().HasForeignKey(x => x.ListingId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.SellerId).OnDelete(DeleteBehavior.Restrict);
    }
}
