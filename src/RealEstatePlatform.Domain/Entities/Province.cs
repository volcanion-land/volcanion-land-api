using RealEstatePlatform.Domain.Common;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Province entity
/// </summary>
public class Province : BaseEntity
{
    /// <summary>
    /// Province code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Province name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Province name in English
    /// </summary>
    public string? NameEn { get; set; }

    /// <summary>
    /// Full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Display order
    /// </summary>
    public int DisplayOrder { get; set; }

    // Navigation properties
    public virtual ICollection<District> Districts { get; set; } = new List<District>();
}
