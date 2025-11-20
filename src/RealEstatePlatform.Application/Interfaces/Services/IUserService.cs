using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.User;

namespace RealEstatePlatform.Application.Interfaces.Services;

/// <summary>
/// User service interface
/// </summary>
public interface IUserService
{
    Task<UserProfileDto?> GetUserProfileAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserProfileAsync(string userId, UpdateProfileDto dto, CancellationToken cancellationToken = default);
    Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> FollowUserAsync(string followerId, string followingId, CancellationToken cancellationToken = default);
    Task<bool> UnfollowUserAsync(string followerId, string followingId, CancellationToken cancellationToken = default);
    Task<List<UserProfileDto>> GetFollowingUsersAsync(string userId, CancellationToken cancellationToken = default);
    Task<PaginatedList<UserProfileDto>> SearchUsersAsync(string? keyword, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
}
