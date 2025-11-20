using RealEstatePlatform.Domain.Common;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Conversation participant entity
/// </summary>
public class ConversationParticipant : BaseEntity
{
    /// <summary>
    /// Conversation ID
    /// </summary>
    public Guid ConversationId { get; set; }

    /// <summary>
    /// User ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Last read timestamp
    /// </summary>
    public DateTime? LastReadAt { get; set; }

    // Navigation properties
    public virtual Conversation Conversation { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
}
