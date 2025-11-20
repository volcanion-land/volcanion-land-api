using RealEstatePlatform.Domain.Common;
using RealEstatePlatform.Domain.Enums;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Payment transaction entity
/// </summary>
public class PaymentTransaction : BaseEntity
{
    /// <summary>
    /// Transaction code/reference
    /// </summary>
    public string TransactionCode { get; set; } = string.Empty;

    /// <summary>
    /// User ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Listing package ID
    /// </summary>
    public Guid ListingPackageId { get; set; }

    /// <summary>
    /// Amount
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    public string Currency { get; set; } = "VND";

    /// <summary>
    /// Payment status
    /// </summary>
    public PaymentStatus Status { get; set; }

    /// <summary>
    /// Payment method (e.g., bank transfer, credit card)
    /// </summary>
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// Payment gateway (e.g., VNPay, Momo)
    /// </summary>
    public string? PaymentGateway { get; set; }

    /// <summary>
    /// Gateway transaction ID
    /// </summary>
    public string? GatewayTransactionId { get; set; }

    /// <summary>
    /// Payment date
    /// </summary>
    public DateTime? PaymentDate { get; set; }

    /// <summary>
    /// Description/notes
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Additional metadata as JSON
    /// </summary>
    public string? Metadata { get; set; }

    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ListingPackage ListingPackage { get; set; } = null!;
}
