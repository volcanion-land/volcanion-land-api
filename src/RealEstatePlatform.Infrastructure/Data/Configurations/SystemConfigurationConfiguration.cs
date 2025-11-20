using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Infrastructure.Data.Configurations;

public class SystemConfigurationConfiguration : IEntityTypeConfiguration<SystemConfiguration>
{
    public void Configure(EntityTypeBuilder<SystemConfiguration> builder)
    {
        builder.ToTable("SystemConfigurations");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Key)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Value)
            .IsRequired();

        builder.Property(s => s.Group)
            .HasMaxLength(50);

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.Property(s => s.DataType)
            .HasMaxLength(20);

        // Unique index on Key
        builder.HasIndex(s => s.Key)
            .IsUnique();

        builder.HasIndex(s => s.Group);
    }
}
