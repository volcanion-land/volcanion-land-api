using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Province
/// </summary>
public class ProvinceConfiguration : IEntityTypeConfiguration<Province>
{
    public void Configure(EntityTypeBuilder<Province> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.NameEn)
            .HasMaxLength(200);

        builder.Property(p => p.FullName)
            .IsRequired()
            .HasMaxLength(300);

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.HasIndex(p => p.Code)
            .IsUnique();
        builder.HasIndex(p => p.Name);
    }
}
