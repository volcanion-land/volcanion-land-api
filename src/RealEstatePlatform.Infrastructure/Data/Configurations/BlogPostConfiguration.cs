using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for BlogPost
/// </summary>
public class BlogPostConfiguration : IEntityTypeConfiguration<BlogPost>
{
    public void Configure(EntityTypeBuilder<BlogPost> builder)
    {
        builder.HasKey(bp => bp.Id);

        builder.Property(bp => bp.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(bp => bp.Slug)
            .IsRequired()
            .HasMaxLength(550);

        builder.Property(bp => bp.Excerpt)
            .HasMaxLength(1000);

        builder.Property(bp => bp.Content)
            .IsRequired()
            .HasMaxLength(50000);

        builder.Property(bp => bp.FeaturedImageUrl)
            .HasMaxLength(1000);

        builder.Property(bp => bp.MetaTitle)
            .HasMaxLength(200);

        builder.Property(bp => bp.MetaDescription)
            .HasMaxLength(500);

        builder.Property(bp => bp.MetaKeywords)
            .HasMaxLength(500);

        builder.HasOne(bp => bp.Author)
            .WithMany(u => u.BlogPosts)
            .HasForeignKey(bp => bp.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(bp => bp.Category)
            .WithMany(c => c.BlogPosts)
            .HasForeignKey(bp => bp.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(bp => !bp.IsDeleted);

        builder.HasIndex(bp => bp.Slug)
            .IsUnique();
        builder.HasIndex(bp => bp.AuthorId);
        builder.HasIndex(bp => bp.CategoryId);
        builder.HasIndex(bp => bp.IsPublished);
        builder.HasIndex(bp => bp.PublishedAt);
    }
}
