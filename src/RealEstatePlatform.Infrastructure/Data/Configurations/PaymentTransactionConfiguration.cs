using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for PaymentTransaction
/// </summary>
public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.HasKey(pt => pt.Id);

        builder.Property(pt => pt.TransactionCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(pt => pt.Amount)
            .HasPrecision(18, 2);

        builder.Property(pt => pt.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(pt => pt.PaymentMethod)
            .HasMaxLength(100);

        builder.Property(pt => pt.PaymentGateway)
            .HasMaxLength(100);

        builder.Property(pt => pt.GatewayTransactionId)
            .HasMaxLength(200);

        builder.Property(pt => pt.Description)
            .HasMaxLength(1000);

        builder.Property(pt => pt.Metadata)
            .HasColumnType("jsonb");

        builder.HasOne(pt => pt.User)
            .WithMany(u => u.Transactions)
            .HasForeignKey(pt => pt.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pt => pt.ListingPackage)
            .WithMany(lp => lp.Transactions)
            .HasForeignKey(pt => pt.ListingPackageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(pt => !pt.IsDeleted);

        builder.HasIndex(pt => pt.TransactionCode)
            .IsUnique();
        builder.HasIndex(pt => pt.UserId);
        builder.HasIndex(pt => pt.Status);
        builder.HasIndex(pt => pt.CreatedAt);
    }
}
