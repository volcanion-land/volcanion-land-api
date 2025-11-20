using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Infrastructure.Data.Configurations;

public class FAQConfiguration : IEntityTypeConfiguration<FAQ>
{
    public void Configure(EntityTypeBuilder<FAQ> builder)
    {
        builder.ToTable("FAQs");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Question)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(f => f.Answer)
            .IsRequired();

        builder.Property(f => f.Category)
            .HasMaxLength(50);

        // Indexes
        builder.HasIndex(f => new { f.Category, f.IsPublished, f.DisplayOrder });
        builder.HasIndex(f => f.ViewCount);
    }
}
