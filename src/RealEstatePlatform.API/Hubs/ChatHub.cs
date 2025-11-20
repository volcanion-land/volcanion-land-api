using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace RealEstatePlatform.API.Hubs;

/// <summary>
/// SignalR hub for real-time chat messaging
/// </summary>
[Authorize]
public class ChatHub : Hub
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
    /// Join a conversation
    /// </summary>
    public async Task JoinConversation(string conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        await Clients.Group($"conversation_{conversationId}").SendAsync("UserJoined", Context.User?.FindFirst("sub")?.Value);
    }

    /// <summary>
    /// Leave a conversation
    /// </summary>
    public async Task LeaveConversation(string conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        await Clients.Group($"conversation_{conversationId}").SendAsync("UserLeft", Context.User?.FindFirst("sub")?.Value);
    }

    /// <summary>
    /// Send a message to a conversation
    /// </summary>
    public async Task SendMessage(string conversationId, string message)
    {
        var userId = Context.User?.FindFirst("sub")?.Value;
        var userName = Context.User?.FindFirst("name")?.Value ?? "Unknown";

        await Clients.Group($"conversation_{conversationId}").SendAsync("ReceiveMessage", new
        {
            conversationId,
            senderId = userId,
            senderName = userName,
            message,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Send typing notification
    /// </summary>
    public async Task TypingNotification(string conversationId, bool isTyping)
    {
        var userId = Context.User?.FindFirst("sub")?.Value;
        var userName = Context.User?.FindFirst("name")?.Value ?? "Unknown";

        await Clients.OthersInGroup($"conversation_{conversationId}").SendAsync("UserTyping", new
        {
            userId,
            userName,
            isTyping
        });
    }

    /// <summary>
    /// Send message to specific user
    /// </summary>
    public async Task SendDirectMessage(string receiverId, string message)
    {
        var senderId = Context.User?.FindFirst("sub")?.Value;
        var senderName = Context.User?.FindFirst("name")?.Value ?? "Unknown";

        await Clients.Group($"user_{receiverId}").SendAsync("ReceiveDirectMessage", new
        {
            senderId,
            senderName,
            message,
            timestamp = DateTime.UtcNow
        });
    }
}
