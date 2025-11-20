using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.User;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.API.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin,Moderator")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserManager<ApplicationUser> userManager, ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<UserProfileDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] string? search,
        [FromQuery] string? role,
        [FromQuery] bool? isVerified,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = _userManager.Users.Where(u => !u.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => u.FullName.Contains(search) || u.Email!.Contains(search));
            }

            if (isVerified.HasValue)
            {
                query = query.Where(u => u.EmailConfirmed == isVerified.Value);
            }

            var totalCount = query.Count();
            var users = query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtos = new List<UserProfileDto>();
            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                
                if (!string.IsNullOrWhiteSpace(role) && !userRoles.Contains(role))
                    continue;

                dtos.Add(new UserProfileDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    AvatarUrl = user.AvatarUrl,
                    Bio = user.Bio,
                    IsVerified = user.EmailConfirmed,
                    Roles = userRoles.ToList(),
                    CreatedAt = user.CreatedAt
                });
            }

            var result = new PaginatedList<UserProfileDto>(dtos, totalCount, pageNumber, pageSize);
            return Ok(ApiResponse<PaginatedList<UserProfileDto>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, ApiResponse<PaginatedList<UserProfileDto>>.ErrorResponse("An error occurred"));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserById(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.IsDeleted)
            {
                return NotFound(ApiResponse<UserProfileDto>.ErrorResponse("User not found"));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var dto = new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = user.AvatarUrl,
                Bio = user.Bio,
                IsVerified = user.EmailConfirmed,
                Roles = roles.ToList(),
                CreatedAt = user.CreatedAt
            };

            return Ok(ApiResponse<UserProfileDto>.SuccessResponse(dto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user");
            return StatusCode(500, ApiResponse<UserProfileDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPut("{id}/lock")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LockUser(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.IsDeleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("User not found"));
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to lock user"));
            }

            _logger.LogInformation("User {UserId} locked", id);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "User locked successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error locking user");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPut("{id}/unlock")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UnlockUser(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.IsDeleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("User not found"));
            }

            var result = await _userManager.SetLockoutEndDateAsync(user, null);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to unlock user"));
            }

            _logger.LogInformation("User {UserId} unlocked", id);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "User unlocked successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlocking user");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPut("{id}/roles")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUserRoles(string id, [FromBody] List<string> roles)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.IsDeleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("User not found"));
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to remove existing roles"));
            }

            var addResult = await _userManager.AddToRolesAsync(user, roles);
            if (!addResult.Succeeded)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to add new roles"));
            }

            _logger.LogInformation("Updated roles for user {UserId}", id);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Roles updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user roles");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteUser(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.IsDeleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("User not found"));
            }

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to delete user"));
            }

            _logger.LogInformation("User {UserId} deleted", id);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "User deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }
}
