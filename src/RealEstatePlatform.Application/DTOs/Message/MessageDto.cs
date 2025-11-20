namespace RealEstatePlatform.Application.DTOs.Message;

/// <summary>
/// Message DTO
/// </summary>
public class MessageDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string SenderId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string? SenderAvatar { get; set; }
    public string? SenderAvatarUrl { get; set; }
    public string ReceiverId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime SentAt { get; set; }
}
