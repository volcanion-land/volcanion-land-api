using RealEstatePlatform.Domain.Common;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// District entity
/// </summary>
public class District : BaseEntity
{
    /// <summary>
    /// District code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// District name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// District name in English
    /// </summary>
    public string? NameEn { get; set; }

    /// <summary>
    /// Full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Province ID
    /// </summary>
    public Guid ProvinceId { get; set; }

    /// <summary>
    /// Display order
    /// </summary>
    public int DisplayOrder { get; set; }

    // Navigation properties
    public virtual Province Province { get; set; } = null!;
    public virtual ICollection<Ward> Wards { get; set; } = new List<Ward>();
}
