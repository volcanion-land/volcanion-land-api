using RealEstatePlatform.Domain.Common;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Favorite listing entity
/// </summary>
public class FavoriteListing : BaseEntity
{
    /// <summary>
    /// User ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Property listing ID
    /// </summary>
    public Guid PropertyListingId { get; set; }

    /// <summary>
    /// Listing ID (alias for PropertyListingId)
    /// </summary>
    public Guid ListingId
    {
        get => PropertyListingId;
        set => PropertyListingId = value;
    }

    /// <summary>
    /// Optional note from user
    /// </summary>
    public string? Note { get; set; }

    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual PropertyListing PropertyListing { get; set; } = null!;
    public virtual PropertyListing? Listing => PropertyListing;
}
