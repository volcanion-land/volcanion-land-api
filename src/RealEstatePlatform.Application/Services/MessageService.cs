using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.Message;
using RealEstatePlatform.Application.Interfaces.Repositories;
using RealEstatePlatform.Application.Interfaces.Services;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Application.Services;

public class MessageService : IMessageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MessageService> _logger;

    public MessageService(
        IUnitOfWork unitOfWork,
        ILogger<MessageService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<PaginatedList<ConversationDto>> GetUserConversationsAsync(
        string userId, 
        int pageNumber = 1, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default)
    {
        var allParticipants = await _unitOfWork.ConversationParticipants
            .FindAsync(cp => cp.UserId == userId && !cp.IsDeleted, cancellationToken);

        var participantQuery = allParticipants.AsQueryable()
            .OrderByDescending(cp => cp.CreatedAt);

        var totalCount = await participantQuery.CountAsync(cancellationToken);
        var participants = await participantQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtos = participants
            .Where(p => p.Conversation != null)
            .Select(p =>
            {
                var otherParticipant = p.Conversation!.Participants
                    .FirstOrDefault(cp => cp.UserId != userId);
                var lastMessage = p.Conversation.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .FirstOrDefault();

                return new ConversationDto
                {
                    Id = p.ConversationId,
                    OtherUserId = otherParticipant?.UserId ?? string.Empty,
                    OtherUserName = otherParticipant?.User?.FullName ?? "Unknown",
                    OtherUserAvatar = otherParticipant?.User?.AvatarUrl,
                    LastMessage = lastMessage?.Content,
                    LastMessageTime = lastMessage?.CreatedAt,
                    UnreadCount = p.Conversation.Messages.Count(m => 
                        m.ReceiverId == userId && !m.IsRead)
                };
            }).ToList();

        return new PaginatedList<ConversationDto>(dtos, totalCount, pageNumber, pageSize);
    }

    public async Task<PaginatedList<MessageDto>> GetConversationMessagesAsync(
        Guid conversationId, 
        string userId, 
        int pageNumber = 1, 
        int pageSize = 50, 
        CancellationToken cancellationToken = default)
    {
        // Verify user is participant
        var isParticipant = await _unitOfWork.ConversationParticipants
            .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId && !cp.IsDeleted, 
                cancellationToken);

        if (!isParticipant)
        {
            throw new UnauthorizedAccessException("User is not a participant of this conversation");
        }

        var messages = await _unitOfWork.Messages
            .FindAsync(m => m.ConversationId == conversationId && !m.IsDeleted, cancellationToken);

        var query = messages.AsQueryable()
            .OrderByDescending(m => m.CreatedAt);

        var totalCount = query.Count();
        var pagedMessages = query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dtos = messages.Select(m => new MessageDto
        {
            Id = m.Id,
            ConversationId = m.ConversationId,
            SenderId = m.SenderId,
            SenderName = m.Sender?.FullName ?? "Unknown",
            SenderAvatar = m.Sender?.AvatarUrl,
            ReceiverId = m.ReceiverId,
            Content = m.Content,
            IsRead = m.IsRead,
            ReadAt = m.ReadAt,
            CreatedAt = m.CreatedAt
        }).Reverse().ToList(); // Reverse to show oldest first

        return new PaginatedList<MessageDto>(dtos, totalCount, pageNumber, pageSize);
    }

    public async Task<MessageDto> SendMessageAsync(
        string senderId, 
        string receiverId, 
        string content,
        Guid? relatedListingId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Message content cannot be empty");
        }

        // Find or create conversation
        var conversation = await FindOrCreateConversationAsync(senderId, receiverId, cancellationToken);

        var message = new Message
        {
            ConversationId = conversation.Id,
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content.Trim(),
            IsRead = false
        };

        await _unitOfWork.Messages.AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Load sender info
        var sender = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Id == senderId, cancellationToken);

        var messageDto = new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            SenderName = sender?.FullName ?? "Unknown",
            SenderAvatar = sender?.AvatarUrl,
            ReceiverId = message.ReceiverId,
            Content = message.Content,
            IsRead = message.IsRead,
            ReadAt = message.ReadAt,
            CreatedAt = message.CreatedAt
        };

        // Note: Real-time message should be sent via SignalR at API layer

        _logger.LogInformation("Message sent from {SenderId} to {ReceiverId}", senderId, receiverId);
        return messageDto;
    }

    public async Task MarkConversationAsReadAsync(
        Guid conversationId, 
        string userId, 
        CancellationToken cancellationToken = default)
    {
        var messages = await _unitOfWork.Messages
            .FindAsync(m => m.ConversationId == conversationId && 
                       m.ReceiverId == userId && 
                       !m.IsRead && 
                       !m.IsDeleted, cancellationToken);

        var messageList = messages.ToList();
        if (!messageList.Any())
        {
            return;
        }

        var now = DateTime.UtcNow;
        foreach (var message in messageList)
        {
            message.IsRead = true;
            message.ReadAt = now;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("{Count} messages marked as read in conversation {ConversationId}", 
            messageList.Count, conversationId);
    }

    public async Task<bool> DeleteConversationAsync(
        Guid conversationId, 
        string userId, 
        CancellationToken cancellationToken = default)
    {
        var participant = await _unitOfWork.ConversationParticipants
            .FirstOrDefaultAsync(cp => cp.ConversationId == conversationId && 
                                      cp.UserId == userId && 
                                      !cp.IsDeleted, cancellationToken);

        if (participant == null)
        {
            return false;
        }

        participant.IsDeleted = true;
        participant.DeletedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Conversation {ConversationId} deleted by user {UserId}", conversationId, userId);

        return true;
    }

    private async Task<Conversation> FindOrCreateConversationAsync(
        string userId1, 
        string userId2, 
        CancellationToken cancellationToken)
    {
        // Find existing conversation between these two users
        var user1Participants = await _unitOfWork.ConversationParticipants
            .FindAsync(cp => cp.UserId == userId1 && !cp.IsDeleted, cancellationToken);

        var existingConversation = user1Participants
            .Select(cp => cp.Conversation)
            .FirstOrDefault(c => c != null && c.Participants.Any(p => p.UserId == userId2 && !p.IsDeleted));

        if (existingConversation != null)
        {
            return existingConversation;
        }

        // Create new conversation
        var conversation = new Conversation();
        await _unitOfWork.Conversations.AddAsync(conversation, cancellationToken);

        var participant1 = new ConversationParticipant
        {
            ConversationId = conversation.Id,
            UserId = userId1
        };

        var participant2 = new ConversationParticipant
        {
            ConversationId = conversation.Id,
            UserId = userId2
        };

        await _unitOfWork.ConversationParticipants.AddAsync(participant1, cancellationToken);
        await _unitOfWork.ConversationParticipants.AddAsync(participant2, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("New conversation created between {User1} and {User2}", userId1, userId2);
        return conversation;
    }
}
