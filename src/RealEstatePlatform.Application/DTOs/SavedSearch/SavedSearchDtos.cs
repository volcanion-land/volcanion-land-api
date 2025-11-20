namespace RealEstatePlatform.Application.DTOs.SavedSearch;

public class SavedSearchDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SearchCriteria { get; set; } = string.Empty;
    public bool EmailNotificationEnabled { get; set; }
    public string NotificationFrequency { get; set; } = "daily";
    public DateTime? LastNotificationDate { get; set; }
    public DateTime? LastNotificationSent { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateSavedSearchDto
{
    public string Name { get; set; } = string.Empty;
    public string SearchCriteria { get; set; } = string.Empty;
    public bool EmailNotificationEnabled { get; set; } = true;
    public string NotificationFrequency { get; set; } = "daily";
    public bool IsActive { get; set; } = true;
}

public class UpdateSavedSearchDto
{
    public string Name { get; set; } = string.Empty;
    public string SearchCriteria { get; set; } = string.Empty;
    public bool EmailNotificationEnabled { get; set; }
    public string NotificationFrequency { get; set; } = "daily";
    public bool IsActive { get; set; }
}
