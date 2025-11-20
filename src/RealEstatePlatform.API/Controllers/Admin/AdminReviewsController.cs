using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.Review;
using RealEstatePlatform.Application.Interfaces.Repositories;
using System.Security.Claims;

namespace RealEstatePlatform.API.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin,Moderator")]
public class ReviewsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(IUnitOfWork unitOfWork, ILogger<ReviewsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<ReviewDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllReviews(
        [FromQuery] bool? isApproved,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var reviews = await _unitOfWork.Reviews.FindAsync(
                r => !r.IsDeleted && (!isApproved.HasValue || r.IsApproved == isApproved.Value),
                cancellationToken);

            var query = reviews.AsQueryable().OrderByDescending(r => r.CreatedAt);
            var totalCount = query.Count();
            var pagedReviews = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var dtos = pagedReviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Title = r.Title,
                Content = r.Content,
                ReviewerId = r.ReviewerId,
                ReviewerName = r.Reviewer?.FullName ?? "Unknown",
                ReviewerAvatar = r.Reviewer?.AvatarUrl,
                ReviewedUserId = r.ReviewedUserId,
                ListingId = r.ListingId,
                IsApproved = r.IsApproved,
                HelpfulCount = r.HelpfulCount,
                CreatedAt = r.CreatedAt,
                AdminResponse = r.AdminResponse
            }).ToList();

            var result = new PaginatedList<ReviewDto>(dtos, totalCount, pageNumber, pageSize);
            return Ok(ApiResponse<PaginatedList<ReviewDto>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving reviews");
            return StatusCode(500, ApiResponse<PaginatedList<ReviewDto>>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPut("{id}/approve")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ApproveReview(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id, cancellationToken);
            if (review == null || review.IsDeleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Review not found"));
            }

            review.IsApproved = true;
            review.AdminResponse = $"Approved by {User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")}";
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Review {ReviewId} approved", id);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Review approved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving review");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPut("{id}/reject")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RejectReview(Guid id, [FromBody] string? reason, CancellationToken cancellationToken = default)
    {
        try
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id, cancellationToken);
            if (review == null || review.IsDeleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Review not found"));
            }

            review.IsApproved = false;
            review.AdminResponse = string.IsNullOrWhiteSpace(reason) 
                ? "Rejected by admin" 
                : $"Rejected: {reason}";
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Review {ReviewId} rejected", id);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Review rejected"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting review");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPost("{id}/respond")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RespondToReview(Guid id, [FromBody] string response, CancellationToken cancellationToken = default)
    {
        try
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id, cancellationToken);
            if (review == null || review.IsDeleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Review not found"));
            }

            review.AdminResponse = response;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Admin responded to review {ReviewId}", id);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Response added"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error responding to review");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteReview(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id, cancellationToken);
            if (review == null || review.IsDeleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Review not found"));
            }

            await _unitOfWork.Reviews.DeleteAsync(review);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Review {ReviewId} deleted", id);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Review deleted"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting review");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }
}
