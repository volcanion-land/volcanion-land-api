using RealEstatePlatform.Domain.Common;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Property image entity
/// </summary>
public class PropertyImage : BaseEntity
{
    /// <summary>
    /// Image URL
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Image caption/description
    /// </summary>
    public string? Caption { get; set; }

    /// <summary>
    /// Display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Is this the main/primary image
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// Property ID
    /// </summary>
    public Guid PropertyId { get; set; }

    // Navigation properties
    public virtual Property Property { get; set; } = null!;
}
