using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for PropertyAmenity
/// </summary>
public class PropertyAmenityConfiguration : IEntityTypeConfiguration<PropertyAmenity>
{
    public void Configure(EntityTypeBuilder<PropertyAmenity> builder)
    {
        builder.HasKey(pa => pa.Id);

        builder.Property(pa => pa.Value)
            .HasMaxLength(500);

        builder.HasOne(pa => pa.Property)
            .WithMany(p => p.PropertyAmenities)
            .HasForeignKey(pa => pa.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pa => pa.Amenity)
            .WithMany(a => a.PropertyAmenities)
            .HasForeignKey(pa => pa.AmenityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(pa => !pa.IsDeleted);

        builder.HasIndex(pa => new { pa.PropertyId, pa.AmenityId })
            .IsUnique();
    }
}
