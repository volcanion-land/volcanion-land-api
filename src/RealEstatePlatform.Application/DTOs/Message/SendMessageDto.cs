using System.ComponentModel.DataAnnotations;

namespace RealEstatePlatform.Application.DTOs.Message;

/// <summary>
/// DTO for sending a message
/// </summary>
public class SendMessageDto
{
    [Required]
    public string ReceiverId { get; set; } = string.Empty;

    [Required]
    [MaxLength(5000)]
    public string Content { get; set; } = string.Empty;

    public Guid? ConversationId { get; set; }
    public string? Subject { get; set; }
    public Guid? RelatedListingId { get; set; }
}
