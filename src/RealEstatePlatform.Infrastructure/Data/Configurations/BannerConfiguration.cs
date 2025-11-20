using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Infrastructure.Data.Configurations;

public class BannerConfiguration : IEntityTypeConfiguration<Banner>
{
    public void Configure(EntityTypeBuilder<Banner> builder)
    {
        builder.ToTable("Banners");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(b => b.LinkUrl)
            .HasMaxLength(500);

        builder.Property(b => b.Position)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.Target)
            .HasMaxLength(20);

        builder.Property(b => b.Description)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(b => new { b.Position, b.IsActive, b.DisplayOrder });
        builder.HasIndex(b => new { b.StartDate, b.EndDate });
    }
}
