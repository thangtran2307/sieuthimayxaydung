using Marketplace.Domain.Identities;
using Marketplace.Domain.Listings;
using Marketplace.Domain.ModerationDecisions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Infrastructure.Persistence.Configurations;

internal sealed class ModerationDecisionConfiguration : IEntityTypeConfiguration<ModerationDecision>
{
    public void Configure(EntityTypeBuilder<ModerationDecision> builder)
    {
        builder.ToTable("moderation_decisions");
        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.ListingId).HasColumnName("listing_id");
        builder.Property(x => x.AdminId).HasColumnName("admin_id");
        builder.Property(x => x.Decision).HasColumnName("decision").HasConversion<string>().HasMaxLength(16);
        builder.Property(x => x.Reason).HasColumnName("reason");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("timezone('utc', now())");

        builder.HasIndex(x => x.ListingId);

        builder.HasOne<Listing>().WithMany().HasForeignKey(x => x.ListingId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.AdminId).OnDelete(DeleteBehavior.Restrict);
    }
}
