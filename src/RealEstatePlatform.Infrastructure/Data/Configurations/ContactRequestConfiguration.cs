using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Infrastructure.Data.Configurations;

public class ContactRequestConfiguration : IEntityTypeConfiguration<ContactRequest>
{
    public void Configure(EntityTypeBuilder<ContactRequest> builder)
    {
        builder.ToTable("ContactRequests");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Phone)
            .HasMaxLength(20);

        builder.Property(c => c.Subject)
            .HasMaxLength(200);

        builder.Property(c => c.Message)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(c => c.RequestType)
            .HasMaxLength(50);

        builder.Property(c => c.AdminResponse)
            .HasMaxLength(2000);

        // Relationships
        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.Listing)
            .WithMany()
            .HasForeignKey(c => c.ListingId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.Responder)
            .WithMany()
            .HasForeignKey(c => c.RespondedBy)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(c => new { c.IsRead, c.IsResponded });
        builder.HasIndex(c => c.RequestType);
        builder.HasIndex(c => c.CreatedAt);
        builder.HasIndex(c => c.ListingId);
    }
}
