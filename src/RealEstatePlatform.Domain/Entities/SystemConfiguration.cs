using RealEstatePlatform.Domain.Common;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// System configuration settings
/// </summary>
public class SystemConfiguration : BaseEntity
{
    /// <summary>
    /// Configuration key (unique)
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// Configuration value (can be JSON)
    /// </summary>
    public required string Value { get; set; }

    /// <summary>
    /// Configuration group/category
    /// </summary>
    public string? Group { get; set; }

    /// <summary>
    /// Description of what this configuration does
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Data type (string, int, bool, json)
    /// </summary>
    public string DataType { get; set; } = "string";

    /// <summary>
    /// Is this configuration visible in admin UI
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Is this a system configuration (cannot be deleted)
    /// </summary>
    public bool IsSystem { get; set; }
}
