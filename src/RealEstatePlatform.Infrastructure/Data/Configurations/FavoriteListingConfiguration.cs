using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for FavoriteListing
/// </summary>
public class FavoriteListingConfiguration : IEntityTypeConfiguration<FavoriteListing>
{
    public void Configure(EntityTypeBuilder<FavoriteListing> builder)
    {
        builder.HasKey(fl => fl.Id);

        builder.Property(fl => fl.Note)
            .HasMaxLength(1000);

        builder.HasOne(fl => fl.User)
            .WithMany(u => u.FavoriteListings)
            .HasForeignKey(fl => fl.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fl => fl.PropertyListing)
            .WithMany(pl => pl.Favorites)
            .HasForeignKey(fl => fl.PropertyListingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(fl => !fl.IsDeleted);

        builder.HasIndex(fl => new { fl.UserId, fl.PropertyListingId })
            .IsUnique();
        builder.HasIndex(fl => fl.CreatedAt);
    }
}
