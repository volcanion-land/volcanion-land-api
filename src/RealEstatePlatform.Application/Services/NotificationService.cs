using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.Notification;
using RealEstatePlatform.Application.Interfaces.Repositories;
using RealEstatePlatform.Application.Interfaces.Services;
using RealEstatePlatform.Domain.Entities;
using RealEstatePlatform.Domain.Enums;

namespace RealEstatePlatform.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IUnitOfWork unitOfWork,
        ILogger<NotificationService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task SendNotificationAsync(string userId, string title, string message, NotificationType type, CancellationToken cancellationToken = default)
    {
        await CreateNotificationAsync(userId, type, title, message, null, null, cancellationToken);
    }

    public async Task<PaginatedList<NotificationDto>> GetUserNotificationsAsync(
        string userId, 
        int pageNumber = 1, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default)
    {
        var allNotifications = await _unitOfWork.Notifications.FindAsync(
            n => n.UserId == userId && !n.IsDeleted,
            cancellationToken);

        var query = allNotifications.AsQueryable().OrderByDescending(n => n.CreatedAt);
        var totalCount = query.Count();
        var notifications = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dtos = notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Type = n.Type,
            Title = n.Title,
            Message = n.Message,
            IsRead = n.IsRead,
            RelatedEntityId = n.RelatedEntityId,
            RelatedEntityType = n.RelatedEntityType,
            CreatedAt = n.CreatedAt
        }).ToList();

        return new PaginatedList<NotificationDto>(dtos, totalCount, pageNumber, pageSize);
    }

    public async Task<bool> MarkAsReadAsync(Guid notificationId, string userId, CancellationToken cancellationToken = default)
    {
        var notification = await _unitOfWork.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId, cancellationToken);

        if (notification == null)
        {
            return false;
        }

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Notification {NotificationId} marked as read by {UserId}", notificationId, userId);

        return true;
    }

    public async Task<bool> MarkAllAsReadAsync(string userId, CancellationToken cancellationToken = default)
    {
        var notifications = await _unitOfWork.Notifications.FindAsync(
            n => n.UserId == userId && !n.IsRead && !n.IsDeleted,
            cancellationToken);

        var notificationsList = notifications.ToList();
        if (!notificationsList.Any())
        {
            return false;
        }

        var now = DateTime.UtcNow;
        foreach (var notification in notificationsList)
        {
            notification.IsRead = true;
            notification.ReadAt = now;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Count} notifications marked as read for user {UserId}", notificationsList.Count, userId);

        return true;
    }

    public async Task<Guid> CreateNotificationAsync(
        string userId, 
        NotificationType type, 
        string title, 
        string message,
        Guid? relatedEntityId = null,
        string? relatedEntityType = null,
        CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            IsRead = false,
            RelatedEntityId = relatedEntityId,
            RelatedEntityType = relatedEntityType
        };

        await _unitOfWork.Notifications.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Note: Real-time notification should be sent via SignalR at API layer

        _logger.LogInformation("Notification created for user {UserId}: {Title}", userId, title);
        return notification.Id;
    }

    public async Task<bool> DeleteNotificationAsync(Guid notificationId, string userId, CancellationToken cancellationToken = default)
    {
        var notification = await _unitOfWork.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId, cancellationToken);

        if (notification == null)
        {
            return false;
        }

        notification.IsDeleted = true;
        notification.DeletedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Notification {NotificationId} deleted by {UserId}", notificationId, userId);

        return true;
    }

    public async Task<int> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead && !n.IsDeleted, cancellationToken);
    }
}
