using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.Message;
using RealEstatePlatform.Application.Interfaces.Services;
using System.Security.Claims;

namespace RealEstatePlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(
        IMessageService messageService,
        ILogger<MessagesController> logger)
    {
        _messageService = messageService;
        _logger = logger;
    }

    [HttpGet("conversations")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<ConversationDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConversations(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<PaginatedList<ConversationDto>>.ErrorResponse("User not authenticated"));
            }

            var conversations = await _messageService.GetUserConversationsAsync(
                userId, pageNumber, pageSize, cancellationToken);

            return Ok(ApiResponse<PaginatedList<ConversationDto>>.SuccessResponse(conversations, "Conversations retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving conversations");
            return StatusCode(500, ApiResponse<PaginatedList<ConversationDto>>.ErrorResponse("An error occurred"));
        }
    }

    [HttpGet("{conversationId:guid}/messages")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<MessageDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMessages(
        Guid conversationId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<PaginatedList<MessageDto>>.ErrorResponse("User not authenticated"));
            }

            var messages = await _messageService.GetConversationMessagesAsync(
                conversationId, userId, pageNumber, pageSize, cancellationToken);

            return Ok(ApiResponse<PaginatedList<MessageDto>>.SuccessResponse(messages, "Messages retrieved successfully"));
        }
        catch (UnauthorizedAccessException)
        {
            return StatusCode(403, ApiResponse<PaginatedList<MessageDto>>.ErrorResponse("Access denied"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving messages");
            return StatusCode(500, ApiResponse<PaginatedList<MessageDto>>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<MessageDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<MessageDto>.ErrorResponse("User not authenticated"));
            }

            if (userId == dto.ReceiverId)
            {
                return BadRequest(ApiResponse<MessageDto>.ErrorResponse("Cannot send message to yourself"));
            }

            var message = await _messageService.SendMessageAsync(userId, dto.ReceiverId, dto.Content, dto.RelatedListingId, cancellationToken);

            return CreatedAtAction(nameof(GetMessages), new { conversationId = message.ConversationId },
                ApiResponse<MessageDto>.SuccessResponse(message, "Message sent successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            return StatusCode(500, ApiResponse<MessageDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPut("{conversationId:guid}/read")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAsRead(Guid conversationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User not authenticated"));
            }

            await _messageService.MarkConversationAsReadAsync(conversationId, userId, cancellationToken);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Messages marked as read"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking as read");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    [HttpDelete("{conversationId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteConversation(Guid conversationId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User not authenticated"));
            }

            var success = await _messageService.DeleteConversationAsync(conversationId, userId, cancellationToken);

            if (!success)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Conversation not found"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Conversation deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting conversation");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }
}
