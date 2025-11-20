using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for ListingPackage
/// </summary>
public class ListingPackageConfiguration : IEntityTypeConfiguration<ListingPackage>
{
    public void Configure(EntityTypeBuilder<ListingPackage> builder)
    {
        builder.HasKey(lp => lp.Id);

        builder.Property(lp => lp.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(lp => lp.Description)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(lp => lp.Price)
            .HasPrecision(18, 2);

        builder.Property(lp => lp.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.HasQueryFilter(lp => !lp.IsDeleted);

        builder.HasIndex(lp => lp.IsActive);
        builder.HasIndex(lp => lp.IsFeatured);
    }
}
