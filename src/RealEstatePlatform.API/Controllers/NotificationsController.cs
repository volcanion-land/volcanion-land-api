using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.Notification;
using RealEstatePlatform.Application.Interfaces.Services;
using System.Security.Claims;

namespace RealEstatePlatform.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationService notificationService,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Get user notifications
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<NotificationDto>>), 200)]
    public async Task<IActionResult> GetNotifications([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<PaginatedList<NotificationDto>>.ErrorResponse("User not authenticated"));
            }

            var notifications = await _notificationService.GetUserNotificationsAsync(userId, pageNumber, pageSize);
            return Ok(ApiResponse<PaginatedList<NotificationDto>>.SuccessResponse(notifications));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications");
            return StatusCode(500, ApiResponse<PaginatedList<NotificationDto>>.ErrorResponse("An error occurred"));
        }
    }

    /// <summary>
    /// Get unread notifications count
    /// </summary>
    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<IActionResult> GetUnreadCount()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<int>.ErrorResponse("User not authenticated"));
            }

            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(ApiResponse<int>.SuccessResponse(count));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread count");
            return StatusCode(500, ApiResponse<int>.ErrorResponse("An error occurred"));
        }
    }

    /// <summary>
    /// Mark notification as read
    /// </summary>
    [HttpPut("{id}/read")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User not authenticated"));
            }

            var result = await _notificationService.MarkAsReadAsync(id, userId);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Notification not found"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Notification marked as read"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification as read");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    [HttpPut("read-all")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User not authenticated"));
            }

            var result = await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "All notifications marked as read"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    /// <summary>
    /// Delete notification
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> DeleteNotification(Guid id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User not authenticated"));
            }

            var result = await _notificationService.DeleteNotificationAsync(id, userId);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Notification not found"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Notification deleted"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting notification");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }
}
