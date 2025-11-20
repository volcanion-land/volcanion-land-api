using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for NotificationTemplate
/// </summary>
public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
    public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
    {
        builder.HasKey(nt => nt.Id);

        builder.Property(nt => nt.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(nt => nt.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(nt => nt.Subject)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(nt => nt.Body)
            .IsRequired()
            .HasMaxLength(5000);

        builder.HasQueryFilter(nt => !nt.IsDeleted);

        builder.HasIndex(nt => nt.Code)
            .IsUnique();
    }
}
