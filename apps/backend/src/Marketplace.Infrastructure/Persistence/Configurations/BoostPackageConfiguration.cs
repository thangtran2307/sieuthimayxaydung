using Marketplace.Domain.Promotion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Infrastructure.Persistence.Configurations;

internal sealed class BoostPackageConfiguration : IEntityTypeConfiguration<BoostPackage>
{
    public void Configure(EntityTypeBuilder<BoostPackage> builder)
    {
        builder.Ignore(x => x.DomainEvents);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.Tier).HasConversion<string>().HasMaxLength(16);
        builder.Property(x => x.Active).HasDefaultValue(true);
    }
}
