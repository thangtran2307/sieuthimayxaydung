using Marketplace.Domain.Catalog;
using Marketplace.Domain.Identity;
using Marketplace.Domain.Moderation;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Infrastructure.Persistence.Configurations;

internal sealed class ModerationDecisionConfiguration : IEntityTypeConfiguration<ModerationDecision>
{
    public void Configure(EntityTypeBuilder<ModerationDecision> builder)
    {
        builder.Ignore(x => x.DomainEvents);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.Decision).HasConversion<string>().HasMaxLength(16);
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("timezone('utc', now())");
        builder.HasIndex(x => x.ListingId);
        builder.HasOne<Listing>().WithMany().HasForeignKey(x => x.ListingId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.AdminId).OnDelete(DeleteBehavior.Restrict);
    }
}
