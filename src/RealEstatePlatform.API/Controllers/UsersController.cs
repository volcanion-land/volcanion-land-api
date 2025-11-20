using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.User;
using RealEstatePlatform.Application.Interfaces.Services;
using System.Security.Claims;

namespace RealEstatePlatform.API.Controllers;

/// <summary>
/// Users controller
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), 200)]
    public async Task<IActionResult> GetMyProfile()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<UserProfileDto>.ErrorResponse("User not authenticated"));
            }

            var profile = await _userService.GetUserProfileAsync(userId);
            if (profile == null)
            {
                return NotFound(ApiResponse<UserProfileDto>.ErrorResponse("User not found"));
            }

            return Ok(ApiResponse<UserProfileDto>.SuccessResponse(profile));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile");
            return StatusCode(500, ApiResponse<UserProfileDto>.ErrorResponse("An error occurred"));
        }
    }

    /// <summary>
    /// Get user profile by ID
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), 200)]
    public async Task<IActionResult> GetUserProfile(string id)
    {
        try
        {
            var profile = await _userService.GetUserProfileAsync(id);
            if (profile == null)
            {
                return NotFound(ApiResponse<UserProfileDto>.ErrorResponse("User not found"));
            }

            return Ok(ApiResponse<UserProfileDto>.SuccessResponse(profile));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile for {UserId}", id);
            return StatusCode(500, ApiResponse<UserProfileDto>.ErrorResponse("An error occurred"));
        }
    }

    /// <summary>
    /// Update current user profile
    /// </summary>
    [HttpPut("profile")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto dto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User not authenticated"));
            }

            var result = await _userService.UpdateUserProfileAsync(userId, dto);
            if (!result)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to update profile"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Profile updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    /// <summary>
    /// Follow a user
    /// </summary>
    [HttpPost("{id}/follow")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> FollowUser(string id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User not authenticated"));
            }

            var result = await _userService.FollowUserAsync(userId, id);
            if (!result)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Already following this user"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "User followed successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error following user {UserId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    /// <summary>
    /// Unfollow a user
    /// </summary>
    [HttpDelete("{id}/follow")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> UnfollowUser(string id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User not authenticated"));
            }

            var result = await _userService.UnfollowUserAsync(userId, id);
            if (!result)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Not following this user"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "User unfollowed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unfollowing user {UserId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    /// <summary>
    /// Get users that current user is following
    /// </summary>
    [HttpGet("following")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<UserProfileDto>>), 200)]
    public async Task<IActionResult> GetFollowingUsers()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<List<UserProfileDto>>.ErrorResponse("User not authenticated"));
            }

            var users = await _userService.GetFollowingUsersAsync(userId);
            return Ok(ApiResponse<List<UserProfileDto>>.SuccessResponse(users));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting following users");
            return StatusCode(500, ApiResponse<List<UserProfileDto>>.ErrorResponse("An error occurred"));
        }
    }

    /// <summary>
    /// Search users
    /// </summary>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<UserProfileDto>>), 200)]
    public async Task<IActionResult> SearchUsers([FromQuery] string? keyword, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var users = await _userService.SearchUsersAsync(keyword, pageNumber, pageSize);
            return Ok(ApiResponse<PaginatedList<UserProfileDto>>.SuccessResponse(users));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users");
            return StatusCode(500, ApiResponse<PaginatedList<UserProfileDto>>.ErrorResponse("An error occurred"));
        }
    }
}
