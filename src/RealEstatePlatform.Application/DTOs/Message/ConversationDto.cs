namespace RealEstatePlatform.Application.DTOs.Message;

/// <summary>
/// Conversation DTO
/// </summary>
public class ConversationDto
{
    public Guid Id { get; set; }
    public string? OtherUserId { get; set; }
    public string? OtherUserName { get; set; }
    public string? OtherUserAvatar { get; set; }
    public string? Subject { get; set; }
    public string? LastMessage { get; set; }
    public DateTime? LastMessageTime { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public string? LastMessageContent { get; set; }
    public List<ParticipantDto> Participants { get; set; } = new();
    public int UnreadCount { get; set; }
}

public class ParticipantDto
{
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTime? LastReadAt { get; set; }
}
