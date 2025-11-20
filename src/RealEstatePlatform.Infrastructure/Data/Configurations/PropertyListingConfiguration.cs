using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for PropertyListing
/// </summary>
public class PropertyListingConfiguration : IEntityTypeConfiguration<PropertyListing>
{
    public void Configure(EntityTypeBuilder<PropertyListing> builder)
    {
        builder.HasKey(pl => pl.Id);

        builder.Property(pl => pl.Price)
            .HasPrecision(18, 2);

        builder.Property(pl => pl.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(pl => pl.ContactName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(pl => pl.ContactPhone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(pl => pl.ContactEmail)
            .HasMaxLength(100);

        builder.Property(pl => pl.Notes)
            .HasMaxLength(2000);

        builder.HasOne(pl => pl.Property)
            .WithMany(p => p.Listings)
            .HasForeignKey(pl => pl.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pl => pl.User)
            .WithMany(u => u.Listings)
            .HasForeignKey(pl => pl.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pl => pl.ListingPackage)
            .WithMany(lp => lp.Listings)
            .HasForeignKey(pl => pl.ListingPackageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(pl => !pl.IsDeleted);

        builder.HasIndex(pl => pl.Status);
        builder.HasIndex(pl => pl.ListingType);
        builder.HasIndex(pl => pl.PropertyId);
        builder.HasIndex(pl => pl.UserId);
        builder.HasIndex(pl => pl.IsFeatured);
        builder.HasIndex(pl => pl.CreatedAt);
        builder.HasIndex(pl => new { pl.Status, pl.ListingType });
    }
}
