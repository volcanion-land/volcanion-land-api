using RealEstatePlatform.Domain.Common;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Conversation entity for messaging
/// </summary>
public class Conversation : BaseEntity
{
    /// <summary>
    /// Conversation subject/title
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Last message timestamp
    /// </summary>
    public DateTime? LastMessageAt { get; set; }

    // Navigation properties
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    public virtual ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
}
