using RealEstatePlatform.Domain.Common;
using RealEstatePlatform.Domain.Enums;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Property listing entity for sale or rent
/// </summary>
public class PropertyListing : BaseEntity
{
    /// <summary>
    /// Type of listing (sale or rent)
    /// </summary>
    public ListingType ListingType { get; set; }

    /// <summary>
    /// Listing price
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Currency code (e.g., VND, USD)
    /// </summary>
    public string Currency { get; set; } = "VND";

    /// <summary>
    /// Listing status
    /// </summary>
    public ListingStatus Status { get; set; }

    /// <summary>
    /// Date when listing becomes active
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Date when listing expires
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// View count
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// Contact name
    /// </summary>
    public string ContactName { get; set; } = string.Empty;

    /// <summary>
    /// Contact phone
    /// </summary>
    public string ContactPhone { get; set; } = string.Empty;

    /// <summary>
    /// Contact email
    /// </summary>
    public string? ContactEmail { get; set; }

    /// <summary>
    /// Additional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Is listing featured/premium
    /// </summary>
    public bool IsFeatured { get; set; }

    /// <summary>
    /// Property ID
    /// </summary>
    public Guid PropertyId { get; set; }

    /// <summary>
    /// Listing owner user ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Listing package ID (optional)
    /// </summary>
    public Guid? ListingPackageId { get; set; }

    // Navigation properties
    public virtual Property Property { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual ListingPackage? ListingPackage { get; set; }
    public virtual ICollection<FavoriteListing> Favorites { get; set; } = new List<FavoriteListing>();
    public virtual ICollection<ViewHistory> ViewHistories { get; set; } = new List<ViewHistory>();
}
