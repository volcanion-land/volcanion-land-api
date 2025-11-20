using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.User;
using RealEstatePlatform.Application.Interfaces.Repositories;
using RealEstatePlatform.Application.Interfaces.Services;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        ILogger<UserService> logger)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<UserProfileDto?> GetUserProfileAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            AvatarUrl = user.AvatarUrl,
            DateOfBirth = user.DateOfBirth,
            Address = user.Address,
            Bio = user.Bio,
            IsVerified = user.EmailConfirmed,
            Roles = roles.ToList(),
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<bool> UpdateUserProfileAsync(string userId, UpdateProfileDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;

        user.FullName = dto.FullName;
        user.PhoneNumber = dto.PhoneNumber;
        user.AvatarUrl = dto.AvatarUrl;
        user.DateOfBirth = dto.DateOfBirth;
        user.Address = dto.Address;
        user.Bio = dto.Bio;

        var result = await _userManager.UpdateAsync(user);
        
        if (result.Succeeded)
        {
            _logger.LogInformation("User profile updated for {UserId}", userId);
        }

        return result.Succeeded;
    }

    public async Task<bool> FollowUserAsync(string followerId, string followingId, CancellationToken cancellationToken = default)
    {
        if (followerId == followingId)
        {
            throw new InvalidOperationException("Cannot follow yourself");
        }

        var existing = await _unitOfWork.UserFollowings
            .FirstOrDefaultAsync(uf => uf.FollowerId == followerId && uf.FollowingId == followingId, cancellationToken);

        if (existing != null)
        {
            return false; // Already following
        }

        var following = new UserFollowing
        {
            FollowerId = followerId,
            FollowingId = followingId
        };

        await _unitOfWork.UserFollowings.AddAsync(following, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {FollowerId} is now following {FollowingId}", followerId, followingId);
        return true;
    }

    public async Task<bool> UserExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user != null && !user.IsDeleted;
    }

    public async Task<bool> UnfollowUserAsync(string followerId, string followingId, CancellationToken cancellationToken = default)
    {
        var following = await _unitOfWork.UserFollowings
            .FirstOrDefaultAsync(uf => uf.FollowerId == followerId && uf.FollowingId == followingId, cancellationToken);

        if (following == null)
        {
            return false;
        }

        await _unitOfWork.UserFollowings.DeleteAsync(following);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User {FollowerId} unfollowed {FollowingId}", followerId, followingId);
        return true;
    }

    public async Task<List<UserProfileDto>> GetFollowingUsersAsync(string userId, CancellationToken cancellationToken = default)
    {
        var followings = await _unitOfWork.UserFollowings
            .FindAsync(uf => uf.FollowerId == userId && !uf.IsDeleted, cancellationToken);

        var users = new List<UserProfileDto>();
        foreach (var following in followings)
        {
            if (following.Following != null)
            {
                var roles = await _userManager.GetRolesAsync(following.Following);
                users.Add(new UserProfileDto
                {
                    Id = following.Following.Id,
                    Email = following.Following.Email ?? string.Empty,
                    FullName = following.Following.FullName,
                    PhoneNumber = following.Following.PhoneNumber,
                    AvatarUrl = following.Following.AvatarUrl,
                    Bio = following.Following.Bio,
                    IsVerified = following.Following.EmailConfirmed,
                    Roles = roles.ToList()
                });
            }
        }

        return users;
    }

    public async Task<PaginatedList<UserProfileDto>> SearchUsersAsync(string? keyword, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = _userManager.Users.Where(u => !u.IsDeleted);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(u => 
                u.FullName.Contains(keyword) || 
                u.Email!.Contains(keyword) ||
                (u.PhoneNumber != null && u.PhoneNumber.Contains(keyword)));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var users = await query
            .OrderBy(u => u.FullName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var userDtos = new List<UserProfileDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = user.AvatarUrl,
                Bio = user.Bio,
                IsVerified = user.EmailConfirmed,
                Roles = roles.ToList()
            });
        }

        return new PaginatedList<UserProfileDto>(userDtos, totalCount, pageNumber, pageSize);
    }
}
