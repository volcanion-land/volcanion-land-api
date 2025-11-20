using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.Review;
using RealEstatePlatform.Application.Interfaces.Services;
using System.Security.Claims;

namespace RealEstatePlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
    {
        _reviewService = reviewService;
        _logger = logger;
    }

    [HttpGet("listing/{listingId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<ReviewDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListingReviews(Guid listingId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            var reviews = await _reviewService.GetListingReviewsAsync(listingId, pageNumber, pageSize, cancellationToken);
            return Ok(ApiResponse<PaginatedList<ReviewDto>>.SuccessResponse(reviews));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving listing reviews");
            return StatusCode(500, ApiResponse<PaginatedList<ReviewDto>>.ErrorResponse("An error occurred"));
        }
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<ReviewDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserReviews(string userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            var reviews = await _reviewService.GetUserReviewsAsync(userId, pageNumber, pageSize, cancellationToken);
            return Ok(ApiResponse<PaginatedList<ReviewDto>>.SuccessResponse(reviews));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user reviews");
            return StatusCode(500, ApiResponse<PaginatedList<ReviewDto>>.ErrorResponse("An error occurred"));
        }
    }

    [HttpGet("listing/{listingId:guid}/rating")]
    [ProducesResponseType(typeof(ApiResponse<double>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListingRating(Guid listingId, CancellationToken cancellationToken = default)
    {
        try
        {
            var rating = await _reviewService.GetAverageRatingAsync(listingId, null, cancellationToken);
            return Ok(ApiResponse<double>.SuccessResponse(rating));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving listing rating");
            return StatusCode(500, ApiResponse<double>.ErrorResponse("An error occurred"));
        }
    }

    [HttpGet("user/{userId}/rating")]
    [ProducesResponseType(typeof(ApiResponse<double>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserRating(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var rating = await _reviewService.GetAverageRatingAsync(null, userId, cancellationToken);
            return Ok(ApiResponse<double>.SuccessResponse(rating));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user rating");
            return StatusCode(500, ApiResponse<double>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<Guid>.ErrorResponse("User not authenticated"));
            }

            var reviewId = await _reviewService.CreateReviewAsync(userId, dto, cancellationToken);

            return CreatedAtAction(nameof(GetListingReviews), new { listingId = dto.ListingId },
                ApiResponse<Guid>.SuccessResponse(reviewId, "Review created. Pending approval"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating review");
            return StatusCode(500, ApiResponse<Guid>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateReview(Guid id, [FromBody] UpdateReviewDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User not authenticated"));
            }

            var success = await _reviewService.UpdateReviewAsync(id, userId, dto, cancellationToken);

            if (!success)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Review not found"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Review updated"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating review");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteReview(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User not authenticated"));
            }

            var success = await _reviewService.DeleteReviewAsync(id, userId, cancellationToken);

            if (!success)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Review not found"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Review deleted"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting review");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPost("{id:guid}/helpful")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAsHelpful(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var success = await _reviewService.MarkReviewAsHelpfulAsync(id, cancellationToken);

            if (!success)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Review not found"));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Marked as helpful"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking review as helpful");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }
}
