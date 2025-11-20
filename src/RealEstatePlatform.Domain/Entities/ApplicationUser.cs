using Microsoft.AspNetCore.Identity;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Application user entity extending Identity user
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// User's full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// User's avatar URL
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// User's date of birth
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// User's address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// User's bio/description
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// Is user verified (e.g., email verified)
    /// </summary>
    public bool IsVerified { get; set; }

    /// <summary>
    /// Date when user was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date when user was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Soft delete flag
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Date when user was deleted
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Refresh token for JWT authentication
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Refresh token expiry time
    /// </summary>
    public DateTime? RefreshTokenExpiryTime { get; set; }

    // Navigation properties
    public virtual ICollection<Property> Properties { get; set; } = new List<Property>();
    public virtual ICollection<PropertyListing> Listings { get; set; } = new List<PropertyListing>();
    public virtual ICollection<FavoriteListing> FavoriteListings { get; set; } = new List<FavoriteListing>();
    public virtual ICollection<ViewHistory> ViewHistories { get; set; } = new List<ViewHistory>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public virtual ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
    public virtual ICollection<PaymentTransaction> Transactions { get; set; } = new List<PaymentTransaction>();
    public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
}
