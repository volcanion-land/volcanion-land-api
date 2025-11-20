using RealEstatePlatform.Domain.Common;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Ward entity
/// </summary>
public class Ward : BaseEntity
{
    /// <summary>
    /// Ward code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Ward name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Ward name in English
    /// </summary>
    public string? NameEn { get; set; }

    /// <summary>
    /// Full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// District ID
    /// </summary>
    public Guid DistrictId { get; set; }

    /// <summary>
    /// Display order
    /// </summary>
    public int DisplayOrder { get; set; }

    // Navigation properties
    public virtual District District { get; set; } = null!;
    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
}
