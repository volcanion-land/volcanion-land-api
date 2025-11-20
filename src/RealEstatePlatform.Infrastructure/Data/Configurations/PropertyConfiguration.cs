using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Property
/// </summary>
public class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(5000);

        builder.Property(p => p.Area)
            .HasPrecision(18, 2);

        builder.Property(p => p.Address)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.OwnerId)
            .IsRequired();

        builder.HasOne(p => p.Owner)
            .WithMany(u => u.Properties)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Ward)
            .WithMany(w => w.Properties)
            .HasForeignKey(p => p.WardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.HasIndex(p => p.PropertyType);
        builder.HasIndex(p => p.WardId);
        builder.HasIndex(p => p.OwnerId);
        builder.HasIndex(p => p.CreatedAt);
        builder.HasIndex(p => p.Location)
            .HasMethod("GIST");
    }
}
