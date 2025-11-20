using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Ward
/// </summary>
public class WardConfiguration : IEntityTypeConfiguration<Ward>
{
    public void Configure(EntityTypeBuilder<Ward> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.NameEn)
            .HasMaxLength(200);

        builder.Property(w => w.FullName)
            .IsRequired()
            .HasMaxLength(300);

        builder.HasOne(w => w.District)
            .WithMany(d => d.Wards)
            .HasForeignKey(w => w.DistrictId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(w => !w.IsDeleted);

        builder.HasIndex(w => w.Code)
            .IsUnique();
        builder.HasIndex(w => w.DistrictId);
        builder.HasIndex(w => w.Name);
    }
}
