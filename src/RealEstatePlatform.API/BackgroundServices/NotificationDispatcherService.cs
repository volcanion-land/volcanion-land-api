using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RealEstatePlatform.API.Hubs;
using RealEstatePlatform.Domain.Entities;
using RealEstatePlatform.Infrastructure.Data;

namespace RealEstatePlatform.API.BackgroundServices;

/// <summary>
/// Background service to process queued notifications
/// </summary>
public class NotificationDispatcherService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationDispatcherService> _logger;
    private readonly TimeSpan _processingInterval = TimeSpan.FromSeconds(5);

    public NotificationDispatcherService(
        IServiceProvider serviceProvider,
        ILogger<NotificationDispatcherService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Notification Dispatcher Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingNotifications(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Notification Dispatcher Service");
            }

            await Task.Delay(_processingInterval, stoppingToken);
        }

        _logger.LogInformation("Notification Dispatcher Service stopped");
    }

    private async Task ProcessPendingNotifications(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

        // Get unread notifications created in the last minute (simplified queue)
        var pendingNotifications = await context.Notifications
            .Where(n => 
                !n.IsRead && 
                n.CreatedAt > DateTime.UtcNow.AddMinutes(-1))
            .Take(50)
            .ToListAsync(cancellationToken);

        foreach (var notification in pendingNotifications)
        {
            try
            {
                // Send real-time notification via SignalR
                await hubContext.Clients
                    .Group($"user_{notification.UserId}")
                    .SendAsync("ReceiveNotification", new
                    {
                        id = notification.Id,
                        title = notification.Title,
                        message = notification.Message,
                        type = notification.Type.ToString(),
                        relatedEntityId = notification.RelatedEntityId,
                        relatedEntityType = notification.RelatedEntityType,
                        createdAt = notification.CreatedAt
                    }, cancellationToken);

                _logger.LogDebug("Sent notification {NotificationId} to user {UserId}", 
                    notification.Id, notification.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification {NotificationId}", notification.Id);
            }
        }
    }
}
