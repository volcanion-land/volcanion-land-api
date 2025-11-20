using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Infrastructure.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.ReviewerId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(r => r.Rating)
            .IsRequired();

        builder.Property(r => r.Title)
            .HasMaxLength(200);

        builder.Property(r => r.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(r => r.AdminResponse)
            .HasMaxLength(1000);

        // Relationships
        builder.HasOne(r => r.Reviewer)
            .WithMany()
            .HasForeignKey(r => r.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Listing)
            .WithMany()
            .HasForeignKey(r => r.ListingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.ReviewedUser)
            .WithMany()
            .HasForeignKey(r => r.ReviewedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.ParentReview)
            .WithMany(r => r.Replies)
            .HasForeignKey(r => r.ParentReviewId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(r => r.ReviewerId);
        builder.HasIndex(r => r.ListingId);
        builder.HasIndex(r => r.ReviewedUserId);
        builder.HasIndex(r => new { r.Rating, r.IsApproved });
        builder.HasIndex(r => r.CreatedAt);
    }
}
