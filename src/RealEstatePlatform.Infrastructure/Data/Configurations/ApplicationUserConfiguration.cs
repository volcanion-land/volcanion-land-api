using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for ApplicationUser
/// </summary>
public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.AvatarUrl)
            .HasMaxLength(500);

        builder.Property(u => u.Address)
            .HasMaxLength(500);

        builder.Property(u => u.Bio)
            .HasMaxLength(1000);

        builder.HasQueryFilter(u => !u.IsDeleted);

        builder.HasIndex(u => u.Email);
        builder.HasIndex(u => u.UserName);
        builder.HasIndex(u => u.CreatedAt);
    }
}
