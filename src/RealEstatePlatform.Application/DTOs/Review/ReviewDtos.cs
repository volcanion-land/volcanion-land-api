namespace RealEstatePlatform.Application.DTOs.Review;

public class ReviewDto
{
    public Guid Id { get; set; }
    public string ReviewerId { get; set; } = string.Empty;
    public string ReviewerName { get; set; } = string.Empty;
    public string? ReviewerAvatar { get; set; }
    public Guid? ListingId { get; set; }
    public string? ListingTitle { get; set; }
    public string? ReviewedUserId { get; set; }
    public string? ReviewedUserName { get; set; }
    public int Rating { get; set; }
    public string? Title { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public bool IsApproved { get; set; }
    public string? AdminResponse { get; set; }
    public int HelpfulCount { get; set; }
    public Guid? ParentReviewId { get; set; }
    public List<ReviewDto>? Replies { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReviewDto
{
    public Guid? ListingId { get; set; }
    public string? ReviewedUserId { get; set; }
    public int Rating { get; set; }
    public string? Title { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ParentReviewId { get; set; }
}

public class UpdateReviewDto
{
    public int Rating { get; set; }
    public string? Title { get; set; }
    public string Content { get; set; } = string.Empty;
}

public class AdminReviewResponseDto
{
    public string AdminResponse { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
}
