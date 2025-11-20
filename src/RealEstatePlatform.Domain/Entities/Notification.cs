using RealEstatePlatform.Domain.Common;
using RealEstatePlatform.Domain.Enums;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Notification entity
/// </summary>
public class Notification : BaseEntity
{
    /// <summary>
    /// User ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Notification title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Notification message/content
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Notification type
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    /// Is notification read
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Read at timestamp
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Related entity ID (optional)
    /// </summary>
    public Guid? RelatedEntityId { get; set; }

    /// <summary>
    /// Related entity type (optional)
    /// </summary>
    public string? RelatedEntityType { get; set; }

    /// <summary>
    /// Additional data as JSON
    /// </summary>
    public string? Data { get; set; }

    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
}
