using RealEstatePlatform.Domain.Common;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Message entity
/// </summary>
public class Message : BaseEntity
{
    /// <summary>
    /// Conversation ID
    /// </summary>
    public Guid ConversationId { get; set; }

    /// <summary>
    /// Sender user ID
    /// </summary>
    public string SenderId { get; set; } = string.Empty;

    /// <summary>
    /// Receiver user ID
    /// </summary>
    public string ReceiverId { get; set; } = string.Empty;

    /// <summary>
    /// Message content
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Is message read
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Read at timestamp
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Sent at timestamp
    /// </summary>
    public DateTime SentAt { get; set; }

    // Navigation properties
    public virtual Conversation Conversation { get; set; } = null!;
    public virtual ApplicationUser Sender { get; set; } = null!;
    public virtual ApplicationUser Receiver { get; set; } = null!;
}
