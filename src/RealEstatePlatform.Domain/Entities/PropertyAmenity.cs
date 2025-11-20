using RealEstatePlatform.Domain.Common;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Junction entity for Property and Amenity many-to-many relationship
/// </summary>
public class PropertyAmenity : BaseEntity
{
    /// <summary>
    /// Property ID
    /// </summary>
    public Guid PropertyId { get; set; }

    /// <summary>
    /// Amenity ID
    /// </summary>
    public Guid AmenityId { get; set; }

    /// <summary>
    /// Additional value/description for this amenity
    /// </summary>
    public string? Value { get; set; }

    // Navigation properties
    public virtual Property Property { get; set; } = null!;
    public virtual Amenity Amenity { get; set; } = null!;
}
