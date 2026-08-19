using Marketplace.Domain.BoostPackages;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Infrastructure.Persistence.Configurations;

internal sealed class BoostPackageConfiguration : IEntityTypeConfiguration<BoostPackage>
{
    public void Configure(EntityTypeBuilder<BoostPackage> builder)
    {
        builder.ToTable("boost_packages");
        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.Tier).HasColumnName("tier").HasConversion<string>().HasMaxLength(16);
        builder.Property(x => x.Name).HasColumnName("name");
        builder.Property(x => x.PriorityLevel).HasColumnName("priority_level");
        builder.Property(x => x.DurationDays).HasColumnName("duration_days");
        builder.Property(x => x.PriceAmount).HasColumnName("price_amount");
        builder.Property(x => x.Active).HasColumnName("active").HasDefaultValue(true);
    }
}
