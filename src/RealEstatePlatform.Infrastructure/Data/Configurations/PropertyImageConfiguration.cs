using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for PropertyImage
/// </summary>
public class PropertyImageConfiguration : IEntityTypeConfiguration<PropertyImage>
{
    public void Configure(EntityTypeBuilder<PropertyImage> builder)
    {
        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.ImageUrl)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(pi => pi.Caption)
            .HasMaxLength(500);

        builder.HasOne(pi => pi.Property)
            .WithMany(p => p.Images)
            .HasForeignKey(pi => pi.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(pi => !pi.IsDeleted);

        builder.HasIndex(pi => pi.PropertyId);
        builder.HasIndex(pi => pi.DisplayOrder);
    }
}
