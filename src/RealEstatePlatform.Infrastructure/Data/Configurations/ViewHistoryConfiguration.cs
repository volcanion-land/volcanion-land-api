using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for ViewHistory
/// </summary>
public class ViewHistoryConfiguration : IEntityTypeConfiguration<ViewHistory>
{
    public void Configure(EntityTypeBuilder<ViewHistory> builder)
    {
        builder.HasKey(vh => vh.Id);

        builder.Property(vh => vh.IpAddress)
            .HasMaxLength(50);

        builder.Property(vh => vh.UserAgent)
            .HasMaxLength(500);

        builder.HasOne(vh => vh.User)
            .WithMany(u => u.ViewHistories)
            .HasForeignKey(vh => vh.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(vh => vh.PropertyListing)
            .WithMany(pl => pl.ViewHistories)
            .HasForeignKey(vh => vh.PropertyListingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(vh => !vh.IsDeleted);

        builder.HasIndex(vh => vh.PropertyListingId);
        builder.HasIndex(vh => vh.UserId);
        builder.HasIndex(vh => vh.ViewedAt);
    }
}
