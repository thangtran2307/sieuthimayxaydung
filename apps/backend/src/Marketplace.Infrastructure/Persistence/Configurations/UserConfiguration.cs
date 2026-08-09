using Marketplace.Domain.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Ignore(x => x.DomainEvents);
        builder.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.Role).HasConversion<string>().HasMaxLength(16);
        builder.Property(x => x.Email).IsRequired();
        builder.Property(x => x.DisplayName).IsRequired();
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("timezone('utc', now())");
        builder.Property(x => x.UpdatedAt).HasDefaultValueSql("timezone('utc', now())");
        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.GoogleId).IsUnique();
        builder.HasIndex(x => x.Role);
    }
}
