namespace RealEstatePlatform.Application.DTOs.Contact;

public class ContactRequestDto
{
    public Guid Id { get; set; }
    public string? UserId { get; set; }
    public Guid? ListingId { get; set; }
    public string? ListingTitle { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Subject { get; set; }
    public string Message { get; set; } = string.Empty;
    public string RequestType { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public bool IsResponded { get; set; }
    public string? AdminResponse { get; set; }
    public Guid? RelatedListingId { get; set; }
    public DateTime? ResponseDate { get; set; }
    public DateTime? RespondedAt { get; set; }
    public string? RespondedByName { get; set; }
    public string? RespondedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateContactRequestDto
{
    public Guid? ListingId { get; set; }
    public Guid? RelatedListingId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Subject { get; set; }
    public string Message { get; set; } = string.Empty;
    public string RequestType { get; set; } = "general_inquiry";
}

public class RespondContactRequestDto
{
    public string AdminResponse { get; set; } = string.Empty;
}
