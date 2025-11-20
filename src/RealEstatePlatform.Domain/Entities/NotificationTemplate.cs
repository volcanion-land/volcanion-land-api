using RealEstatePlatform.Domain.Common;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Notification template entity
/// </summary>
public class NotificationTemplate : BaseEntity
{
    /// <summary>
    /// Template code/key
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Template name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Template subject
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Template body/content with placeholders
    /// </summary>
    public string Body { get; set; } = string.Empty;

    /// <summary>
    /// Is template active
    /// </summary>
    public bool IsActive { get; set; }
}
