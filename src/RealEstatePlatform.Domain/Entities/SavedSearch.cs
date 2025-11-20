using RealEstatePlatform.Domain.Common;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Saved search criteria for users
/// </summary>
public class SavedSearch : BaseEntity
{
    /// <summary>
    /// User who saved this search
    /// </summary>
    public required string UserId { get; set; }
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// Search name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Search criteria in JSON format
    /// </summary>
    public required string SearchCriteria { get; set; }

    /// <summary>
    /// Email notification enabled
    /// </summary>
    public bool EmailNotificationEnabled { get; set; } = true;

    /// <summary>
    /// Notification frequency (daily, weekly, instant)
    /// </summary>
    public string NotificationFrequency { get; set; } = "daily";

    /// <summary>
    /// Last notification sent date
    /// </summary>
    public DateTime? LastNotificationDate { get; set; }

    /// <summary>
    /// Is this search active
    /// </summary>
    public bool IsActive { get; set; } = true;
}
