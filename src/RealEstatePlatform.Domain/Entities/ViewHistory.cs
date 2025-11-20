using RealEstatePlatform.Domain.Common;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// View history entity to track user views of listings
/// </summary>
public class ViewHistory : BaseEntity
{
    /// <summary>
    /// User ID (nullable for anonymous views)
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Property listing ID
    /// </summary>
    public Guid PropertyListingId { get; set; }

    /// <summary>
    /// IP address of viewer
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent string
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Viewed at timestamp
    /// </summary>
    public DateTime ViewedAt { get; set; }

    // Navigation properties
    public virtual ApplicationUser? User { get; set; }
    public virtual PropertyListing PropertyListing { get; set; } = null!;
}
