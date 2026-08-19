using Marketplace.Domain.Identities;
using Marketplace.Domain.Listings;
using Marketplace.Domain.Reports;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Infrastructure.Persistence.Configurations;

internal sealed class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("reports");
        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.ListingId).HasColumnName("listing_id");
        builder.Property(x => x.Reason).HasColumnName("reason").HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.Details).HasColumnName("details");
        builder.Property(x => x.ReporterContact).HasColumnName("reporter_contact");
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.ResolvedByAdminId).HasColumnName("resolved_by_admin_id");
        builder.Property(x => x.ResolvedAt).HasColumnName("resolved_at");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("timezone('utc', now())");

        builder.HasIndex(x => x.ListingId);
        builder.HasIndex(x => x.Status);

        builder.HasOne<Listing>().WithMany().HasForeignKey(x => x.ListingId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ResolvedByAdminId).OnDelete(DeleteBehavior.Restrict);
    }
}
