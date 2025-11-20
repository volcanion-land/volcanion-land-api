using RealEstatePlatform.Domain.Common;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Review for listing or user (broker)
/// </summary>
public class Review : BaseEntity
{
    /// <summary>
    /// User who wrote the review
    /// </summary>
    public required string ReviewerId { get; set; }
    public ApplicationUser? Reviewer { get; set; }

    /// <summary>
    /// Listing being reviewed (optional)
    /// </summary>
    public Guid? ListingId { get; set; }
    public PropertyListing? Listing { get; set; }

    /// <summary>
    /// User being reviewed (optional - for broker reviews)
    /// </summary>
    public string? ReviewedUserId { get; set; }
    public ApplicationUser? ReviewedUser { get; set; }

    /// <summary>
    /// Rating score (1-5)
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Review title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Review content
    /// </summary>
    public required string Content { get; set; }

    /// <summary>
    /// Is this review verified (user actually engaged in transaction)
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// Is this review approved by admin
    /// </summary>
    public bool IsApproved { get; set; }

    /// <summary>
    /// Admin response to the review
    /// </summary>
    public string? AdminResponse { get; set; }

    /// <summary>
    /// Helpful count
    /// </summary>
    public int HelpfulCount { get; set; }

    /// <summary>
    /// Parent review ID for replies
    /// </summary>
    public Guid? ParentReviewId { get; set; }
    public Review? ParentReview { get; set; }

    /// <summary>
    /// Replies to this review
    /// </summary>
    public ICollection<Review> Replies { get; set; } = new List<Review>();
}
