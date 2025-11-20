namespace RealEstatePlatform.Domain.Common;

/// <summary>
/// Base entity with common auditing fields
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Date and time when entity was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// User ID who created the entity
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Date and time when entity was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// User ID who last updated the entity
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Soft delete flag
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Date and time when entity was deleted
    /// </summary>
    public DateTime? DeletedAt { get; set; }
}
