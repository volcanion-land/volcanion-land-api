using RealEstatePlatform.Domain.Common;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Contact request from users
/// </summary>
public class ContactRequest : BaseEntity
{
    /// <summary>
    /// User who sent the request (optional if anonymous)
    /// </summary>
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    /// <summary>
    /// Listing being inquired about (optional)
    /// </summary>
    public Guid? ListingId { get; set; }
    public PropertyListing? Listing { get; set; }

    /// <summary>
    /// Contact name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Contact email
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Contact phone
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Subject
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Message content
    /// </summary>
    public required string Message { get; set; }

    /// <summary>
    /// Request type (general_inquiry, listing_inquiry, complaint, support)
    /// </summary>
    public string RequestType { get; set; } = "general_inquiry";

    /// <summary>
    /// Is this request read by admin
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Is this request responded
    /// </summary>
    public bool IsResponded { get; set; }

    /// <summary>
    /// Admin response
    /// </summary>
    public string? AdminResponse { get; set; }

    /// <summary>
    /// Response date
    /// </summary>
    public DateTime? ResponseDate { get; set; }

    /// <summary>
    /// Admin who responded
    /// </summary>
    public string? RespondedBy { get; set; }
    public ApplicationUser? Responder { get; set; }
}
