using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.Notification;
using RealEstatePlatform.Domain.Enums;

namespace RealEstatePlatform.Application.Interfaces.Services;

/// <summary>
/// Notification service interface
/// </summary>
public interface INotificationService
{
    Task SendNotificationAsync(string userId, string title, string message, NotificationType type, CancellationToken cancellationToken = default);
    Task<PaginatedList<NotificationDto>> GetUserNotificationsAsync(string userId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<bool> MarkAsReadAsync(Guid notificationId, string userId, CancellationToken cancellationToken = default);
    Task<bool> MarkAllAsReadAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteNotificationAsync(Guid notificationId, string userId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default);
    Task<Guid> CreateNotificationAsync(string userId, NotificationType type, string title, string message, Guid? relatedEntityId = null, string? relatedEntityType = null, CancellationToken cancellationToken = default);
}
