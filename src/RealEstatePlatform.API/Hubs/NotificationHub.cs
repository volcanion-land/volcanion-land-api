using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace RealEstatePlatform.API.Hubs;

/// <summary>
/// SignalR hub for real-time notifications
/// </summary>
[Authorize]
public class NotificationHub : Hub
{
    /// <summary>
    /// Called when a client connects
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst("sub")?.Value;
        
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst("sub")?.Value;
        
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Send notification to specific user
    /// </summary>
    public async Task SendNotificationToUser(string userId, string title, string message, string type)
    {
        await Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", new
        {
            title,
            message,
            type,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    public async Task MarkAsRead(Guid notificationId)
    {
        var userId = Context.User?.FindFirst("sub")?.Value;
        
        if (!string.IsNullOrEmpty(userId))
        {
            await Clients.Caller.SendAsync("NotificationMarkedAsRead", notificationId);
        }
    }
}
