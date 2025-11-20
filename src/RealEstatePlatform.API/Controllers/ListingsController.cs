using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.Listing;
using RealEstatePlatform.Application.Interfaces.Services;
using RealEstatePlatform.Domain.Enums;

namespace RealEstatePlatform.API.Controllers;

/// <summary>
/// Property listings controller
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class ListingsController : ControllerBase
{
    private readonly IPropertyListingService _listingService;
    private readonly ILogger<ListingsController> _logger;

    public ListingsController(
        IPropertyListingService listingService,
        ILogger<ListingsController> logger)
    {
        _listingService = listingService;
        _logger = logger;
    }

    /// <summary>
    /// Search and filter property listings
    /// </summary>
    /// <param name="filter">Search filter parameters</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Paginated list of listings</returns>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<ListingDto>>), 200)]
    public async Task<IActionResult> SearchListings([FromQuery] SearchFilter filter, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _listingService.SearchListingsAsync(filter, cancellationToken);
            return Ok(ApiResponse<PaginatedList<ListingDto>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching listings");
            return StatusCode(500, ApiResponse<PaginatedList<ListingDto>>.ErrorResponse("An error occurred while searching listings"));
        }
    }

    /// <summary>
    /// Get listing detail by ID
    /// </summary>
    /// <param name="id">Listing ID</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Listing details</returns>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<ListingDetailDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetListingDetail(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _listingService.GetListingDetailAsync(id, cancellationToken);
            
            if (result == null)
                return NotFound(ApiResponse<ListingDetailDto>.ErrorResponse("Listing not found"));

            return Ok(ApiResponse<ListingDetailDto>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting listing detail for ID: {ListingId}", id);
            return StatusCode(500, ApiResponse<ListingDetailDto>.ErrorResponse("An error occurred while retrieving listing details"));
        }
    }

    /// <summary>
    /// Create a new property listing
    /// </summary>
    /// <param name="dto">Create listing data</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Created listing ID</returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<Guid>), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> CreateListing([FromBody] CreateListingDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<Guid>.ErrorResponse("User not authenticated"));

            var listingId = await _listingService.CreateListingAsync(dto, userId, cancellationToken);
            
            return CreatedAtAction(
                nameof(GetListingDetail),
                new { id = listingId },
                ApiResponse<Guid>.SuccessResponse(listingId, "Listing created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating listing");
            return StatusCode(500, ApiResponse<Guid>.ErrorResponse("An error occurred while creating the listing"));
        }
    }

    /// <summary>
    /// Update an existing listing
    /// </summary>
    /// <param name="id">Listing ID</param>
    /// <param name="dto">Update listing data</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Success response</returns>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateListing(Guid id, [FromBody] CreateListingDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<object>.ErrorResponse("User not authenticated"));

            await _listingService.UpdateListingAsync(id, dto, userId, cancellationToken);
            
            return Ok(ApiResponse<object?>.SuccessResponse(null, "Listing updated successfully"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Listing not found"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating listing {ListingId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while updating the listing"));
        }
    }

    /// <summary>
    /// Update listing status
    /// </summary>
    /// <param name="id">Listing ID</param>
    /// <param name="status">New status</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Success response</returns>
    [HttpPatch("{id:guid}/status")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateListingStatus(Guid id, [FromBody] ListingStatus status, CancellationToken cancellationToken)
    {
        try
        {
            await _listingService.UpdateListingStatusAsync(id, status, cancellationToken);
            return Ok(ApiResponse<object?>.SuccessResponse(null, "Listing status updated successfully"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Listing not found"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating listing status for ID: {ListingId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while updating listing status"));
        }
    }

    /// <summary>
    /// Delete a listing
    /// </summary>
    /// <param name="id">Listing ID</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Success response</returns>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteListing(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<object>.ErrorResponse("User not authenticated"));

            await _listingService.DeleteListingAsync(id, userId, cancellationToken);
            
            return Ok(ApiResponse<object?>.SuccessResponse(null, "Listing deleted successfully"));
        }
        catch (KeyNotFoundException)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Listing not found"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting listing {ListingId}", id);
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while deleting the listing"));
        }
    }

    /// <summary>
    /// Get current user's listings
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>List of user's listings</returns>
    [HttpGet("my-listings")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ListingDto>>), 200)]
    public async Task<IActionResult> GetMyListings(CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(ApiResponse<IEnumerable<ListingDto>>.ErrorResponse("User not authenticated"));

            var result = await _listingService.GetUserListingsAsync(userId, cancellationToken);
            
            return Ok(ApiResponse<IEnumerable<ListingDto>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user listings");
            return StatusCode(500, ApiResponse<IEnumerable<ListingDto>>.ErrorResponse("An error occurred while retrieving your listings"));
        }
    }

    /// <summary>
    /// Get featured listings
    /// </summary>
    /// <param name="count">Number of featured listings to return</param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of featured listings</returns>
    [HttpGet("featured")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ListingDto>>), 200)]
    public async Task<IActionResult> GetFeaturedListings([FromQuery] int count = 10, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            var result = await _listingService.GetFeaturedListingsAsync(count, cancellationToken);
            return Ok(ApiResponse<IEnumerable<ListingDto>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting featured listings");
            return StatusCode(500, ApiResponse<IEnumerable<ListingDto>>.ErrorResponse("An error occurred while retrieving featured listings"));
        }
    }
}
