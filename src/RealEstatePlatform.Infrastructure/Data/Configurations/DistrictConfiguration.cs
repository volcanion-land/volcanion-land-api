using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for District
/// </summary>
public class DistrictConfiguration : IEntityTypeConfiguration<District>
{
    public void Configure(EntityTypeBuilder<District> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.NameEn)
            .HasMaxLength(200);

        builder.Property(d => d.FullName)
            .IsRequired()
            .HasMaxLength(300);

        builder.HasOne(d => d.Province)
            .WithMany(p => p.Districts)
            .HasForeignKey(d => d.ProvinceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(d => !d.IsDeleted);

        builder.HasIndex(d => d.Code)
            .IsUnique();
        builder.HasIndex(d => d.ProvinceId);
        builder.HasIndex(d => d.Name);
    }
}
