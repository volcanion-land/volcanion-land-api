using RealEstatePlatform.Domain.Common;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// User following relationship (follow brokers/sellers)
/// </summary>
public class UserFollowing : BaseEntity
{
    /// <summary>
    /// User who is following
    /// </summary>
    public required string FollowerId { get; set; }
    public ApplicationUser? Follower { get; set; }

    /// <summary>
    /// User being followed
    /// </summary>
    public required string FollowingId { get; set; }
    public ApplicationUser? Following { get; set; }

    /// <summary>
    /// Notification enabled for new listings from this user
    /// </summary>
    public bool NotificationEnabled { get; set; } = true;
}
