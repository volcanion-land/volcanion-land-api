using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Infrastructure.Data.Configurations;

public class UserFollowingConfiguration : IEntityTypeConfiguration<UserFollowing>
{
    public void Configure(EntityTypeBuilder<UserFollowing> builder)
    {
        builder.ToTable("UserFollowings");

        builder.HasKey(uf => uf.Id);

        builder.Property(uf => uf.FollowerId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(uf => uf.FollowingId)
            .IsRequired()
            .HasMaxLength(450);

        // Relationships
        builder.HasOne(uf => uf.Follower)
            .WithMany()
            .HasForeignKey(uf => uf.FollowerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(uf => uf.Following)
            .WithMany()
            .HasForeignKey(uf => uf.FollowingId)
            .OnDelete(DeleteBehavior.Restrict);

        // Unique constraint - a user can only follow another user once
        builder.HasIndex(uf => new { uf.FollowerId, uf.FollowingId })
            .IsUnique();

        builder.HasIndex(uf => uf.FollowingId);
    }
}
