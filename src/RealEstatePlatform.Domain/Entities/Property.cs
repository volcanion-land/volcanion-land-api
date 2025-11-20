using RealEstatePlatform.Domain.Common;
using RealEstatePlatform.Domain.Enums;
using NetTopologySuite.Geometries;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Property entity representing a real estate property
/// </summary>
public class Property : BaseEntity
{
    /// <summary>
    /// Property title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Property description
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Type of property
    /// </summary>
    public PropertyType PropertyType { get; set; }

    /// <summary>
    /// Property area in square meters
    /// </summary>
    public decimal Area { get; set; }

    /// <summary>
    /// Number of bedrooms
    /// </summary>
    public int? Bedrooms { get; set; }

    /// <summary>
    /// Number of bathrooms
    /// </summary>
    public int? Bathrooms { get; set; }

    /// <summary>
    /// Number of floors
    /// </summary>
    public int? Floors { get; set; }

    /// <summary>
    /// Year built
    /// </summary>
    public int? YearBuilt { get; set; }

    /// <summary>
    /// Street address
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Ward ID
    /// </summary>
    public Guid WardId { get; set; }

    /// <summary>
    /// Latitude and longitude for spatial queries
    /// </summary>
    public Point? Location { get; set; }

    /// <summary>
    /// Property owner user ID
    /// </summary>
    public string OwnerId { get; set; } = string.Empty;

    // Navigation properties
    public virtual ApplicationUser Owner { get; set; } = null!;
    public virtual Ward Ward { get; set; } = null!;
    public virtual ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
    public virtual ICollection<PropertyAmenity> PropertyAmenities { get; set; } = new List<PropertyAmenity>();
    public virtual ICollection<PropertyListing> Listings { get; set; } = new List<PropertyListing>();
}
