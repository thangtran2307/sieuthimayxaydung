using Marketplace.Domain.Identities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.Email).HasColumnName("email").IsRequired();
        builder.Property(x => x.PasswordHash).HasColumnName("password_hash");
        builder.Property(x => x.GoogleId).HasColumnName("google_id");
        builder.Property(x => x.Role).HasColumnName("role").HasConversion<string>().HasMaxLength(16);
        builder.Property(x => x.DisplayName).HasColumnName("display_name").IsRequired();
        builder.Property(x => x.Phone).HasColumnName("phone");
        builder.Property(x => x.LocationProvince).HasColumnName("location_province");
        builder.Property(x => x.Description).HasColumnName("description");
        builder.Property(x => x.CoverImageUrl).HasColumnName("cover_image_url");
        builder.Property(x => x.Verified).HasColumnName("verified");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("timezone('utc', now())");
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("timezone('utc', now())");

        builder.HasIndex(x => x.Email).IsUnique();
        builder.HasIndex(x => x.GoogleId).IsUnique();
        builder.HasIndex(x => x.Role);
    }
}
