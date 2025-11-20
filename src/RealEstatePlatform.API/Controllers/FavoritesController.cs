using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.Favorite;
using RealEstatePlatform.Application.Interfaces.Services;
using System.Security.Claims;

namespace RealEstatePlatform.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
[Produces("application/json")]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;
    private readonly ILogger<FavoritesController> _logger;

    public FavoritesController(
        IFavoriteService favoriteService,
        ILogger<FavoritesController> logger)
    {
        _favoriteService = favoriteService;
        _logger = logger;
    }

    /// <summary>
    /// Get user's favorite listings
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<FavoriteDto>>), 200)]
    public async Task<IActionResult> GetFavorites([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<PaginatedList<FavoriteDto>>.ErrorResponse("User not authenticated"));
            }

            var favorites = await _favoriteService.GetUserFavoritesAsync(userId, pageNumber, pageSize);
            return Ok(ApiResponse<PaginatedList<FavoriteDto>>.SuccessResponse(favorites));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting favorites");
            return StatusCode(500, ApiResponse<PaginatedList<FavoriteDto>>.ErrorResponse("An error occurred"));
        }
    }

    /// <summary>
    /// Add listing to favorites
    /// </summary>
    [HttpPost("{listingId}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> AddToFavorites(Guid listingId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User not authenticated"));
            }

            var result = await _favoriteService.AddToFavoritesAsync(userId, listingId);
            if (!result)
            {
                return BadRequest(ApiResponse<bool>.ErrorResponse("Listing already in favorites"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Added to favorites"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding to favorites");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    /// <summary>
    /// Remove listing from favorites
    /// </summary>
    [HttpDelete("{listingId}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> RemoveFromFavorites(Guid listingId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User not authenticated"));
            }

            var result = await _favoriteService.RemoveFromFavoritesAsync(userId, listingId);
            if (!result)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Listing not in favorites"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Removed from favorites"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing from favorites");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    /// <summary>
    /// Check if listing is favorited
    /// </summary>
    [HttpGet("{listingId}/check")]
    [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
    public async Task<IActionResult> IsFavorited(Guid listingId)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User not authenticated"));
            }

            var result = await _favoriteService.IsListingFavoritedAsync(userId, listingId);
            return Ok(ApiResponse<bool>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking favorite status");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    /// <summary>
    /// Get favorite count for user
    /// </summary>
    [HttpGet("count")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<IActionResult> GetFavoriteCount()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<int>.ErrorResponse("User not authenticated"));
            }

            var count = await _favoriteService.GetFavoriteCountAsync(userId);
            return Ok(ApiResponse<int>.SuccessResponse(count));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting favorite count");
            return StatusCode(500, ApiResponse<int>.ErrorResponse("An error occurred"));
        }
    }
}
