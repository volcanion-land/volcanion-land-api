using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.Message;

namespace RealEstatePlatform.Application.Interfaces.Services;

/// <summary>
/// Message service interface
/// </summary>
public interface IMessageService
{
    Task<PaginatedList<ConversationDto>> GetUserConversationsAsync(string userId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<PaginatedList<MessageDto>> GetConversationMessagesAsync(Guid conversationId, string userId, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);
    Task<MessageDto> SendMessageAsync(string senderId, string receiverId, string content, Guid? relatedListingId = null, CancellationToken cancellationToken = default);
    Task MarkConversationAsReadAsync(Guid conversationId, string userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteConversationAsync(Guid conversationId, string userId, CancellationToken cancellationToken = default);
}
