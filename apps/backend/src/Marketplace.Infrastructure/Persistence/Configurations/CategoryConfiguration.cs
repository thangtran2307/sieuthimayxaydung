using Marketplace.Domain.Categories;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Infrastructure.Persistence.Configurations;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.Slug).HasColumnName("slug");
        builder.Property(x => x.ParentId).HasColumnName("parent_id");
        builder.Property(x => x.LabelVi).HasColumnName("label_vi");
        builder.Property(x => x.LabelEn).HasColumnName("label_en");
        builder.Property(x => x.Icon).HasColumnName("icon");
        builder.Property(x => x.SortOrder).HasColumnName("sort_order");

        builder.HasIndex(x => x.Slug).IsUnique();

        builder.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
