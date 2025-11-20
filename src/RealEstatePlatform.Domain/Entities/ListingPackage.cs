using RealEstatePlatform.Domain.Common;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Listing package entity for premium listing features
/// </summary>
public class ListingPackage : BaseEntity
{
    /// <summary>
    /// Package name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Package description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Package price
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Currency code
    /// </summary>
    public string Currency { get; set; } = "VND";

    /// <summary>
    /// Duration in days
    /// </summary>
    public int DurationDays { get; set; }

    /// <summary>
    /// Number of listings included
    /// </summary>
    public int ListingCount { get; set; }

    /// <summary>
    /// Is featured
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Priority boost value
    /// </summary>
    public int PriorityBoost { get; set; }

    /// <summary>
    /// Is package active
    /// </summary>
    public bool IsActive { get; set; }

    // Navigation properties
    public virtual ICollection<PropertyListing> Listings { get; set; } = new List<PropertyListing>();
    public virtual ICollection<PaymentTransaction> Transactions { get; set; } = new List<PaymentTransaction>();
}
