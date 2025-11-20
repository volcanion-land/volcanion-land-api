using RealEstatePlatform.Domain.Common;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Amenity entity (e.g., swimming pool, gym, parking)
/// </summary>
public class Amenity : BaseEntity
{
    /// <summary>
    /// Amenity name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Amenity description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Icon/image URL
    /// </summary>
    public string? IconUrl { get; set; }

    /// <summary>
    /// Display order
    /// </summary>
    public int DisplayOrder { get; set; }

    // Navigation properties
    public virtual ICollection<PropertyAmenity> PropertyAmenities { get; set; } = new List<PropertyAmenity>();
}
