using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.Listing;
using RealEstatePlatform.Application.Interfaces.Repositories;
using RealEstatePlatform.Domain.Enums;
using System.Security.Claims;

namespace RealEstatePlatform.API.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin,Moderator")]
public class ListingsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ListingsController> _logger;

    public ListingsController(IUnitOfWork unitOfWork, ILogger<ListingsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<ListingDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllListings(
        [FromQuery] ListingStatus? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var listings = await _unitOfWork.PropertyListings.FindAsync(
                l => !l.IsDeleted && (!status.HasValue || l.Status == status.Value),
                cancellationToken);

            var query = listings.AsQueryable().OrderByDescending(l => l.CreatedAt);
            var totalCount = query.Count();
            var pagedListings = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var dtos = pagedListings.Select(l => new ListingDto
            {
                Id = l.Id,
                Title = l.Property?.Title ?? "N/A",
                Address = l.Property?.Address ?? "N/A",
                Price = l.Price,
                Area = l.Property?.Area ?? 0,
                Bedrooms = l.Property?.Bedrooms,
                Bathrooms = l.Property?.Bathrooms,
                Status = l.Status,
                ListingType = l.ListingType,
                PropertyType = l.Property?.PropertyType ?? PropertyType.House,
                CreatedAt = l.CreatedAt,
                IsFeatured = l.IsFeatured,
                ViewCount = l.ViewCount,
                Currency = l.Currency,
                WardName = l.Property?.Ward?.Name ?? string.Empty,
                DistrictName = l.Property?.Ward?.District?.Name ?? string.Empty,
                ProvinceName = l.Property?.Ward?.District?.Province?.Name ?? string.Empty
            }).ToList();

            var result = new PaginatedList<ListingDto>(dtos, totalCount, pageNumber, pageSize);
            return Ok(ApiResponse<PaginatedList<ListingDto>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving listings");
            return StatusCode(500, ApiResponse<PaginatedList<ListingDto>>.ErrorResponse("An error occurred"));
        }
    }

    [HttpGet("pending")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<ListingDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingListings(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var listings = await _unitOfWork.PropertyListings.FindAsync(
                l => !l.IsDeleted && l.Status == ListingStatus.Pending,
                cancellationToken);

            var query = listings.AsQueryable().OrderBy(l => l.CreatedAt);
            var totalCount = query.Count();
            var pagedListings = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var dtos = pagedListings.Select(l => new ListingDto
            {
                Id = l.Id,
                Title = l.Property?.Title ?? "N/A",
                Address = l.Property?.Address ?? "N/A",
                Price = l.Price,
                Area = l.Property?.Area ?? 0,
                Bedrooms = l.Property?.Bedrooms,
                Bathrooms = l.Property?.Bathrooms,
                Status = l.Status,
                ListingType = l.ListingType,
                PropertyType = l.Property?.PropertyType ?? PropertyType.House,
                CreatedAt = l.CreatedAt,
                IsFeatured = l.IsFeatured,
                ViewCount = l.ViewCount,
                Currency = l.Currency,
                WardName = l.Property?.Ward?.Name ?? string.Empty,
                DistrictName = l.Property?.Ward?.District?.Name ?? string.Empty,
                ProvinceName = l.Property?.Ward?.District?.Province?.Name ?? string.Empty
            }).ToList();

            var result = new PaginatedList<ListingDto>(dtos, totalCount, pageNumber, pageSize);
            return Ok(ApiResponse<PaginatedList<ListingDto>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending listings");
            return StatusCode(500, ApiResponse<PaginatedList<ListingDto>>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPut("{id}/approve")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ApproveListing(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var listing = await _unitOfWork.PropertyListings.GetByIdAsync(id, cancellationToken);
            if (listing == null || listing.IsDeleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Listing not found"));
            }

            listing.Status = ListingStatus.Active;
            listing.Notes = $"Approved at {DateTime.UtcNow} by {User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")}";

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Listing {ListingId} approved", id);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Listing approved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving listing");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPut("{id}/reject")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RejectListing(Guid id, [FromBody] string? reason, CancellationToken cancellationToken = default)
    {
        try
        {
            var listing = await _unitOfWork.PropertyListings.GetByIdAsync(id, cancellationToken);
            if (listing == null || listing.IsDeleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Listing not found"));
            }

            listing.Status = ListingStatus.Cancelled;
            listing.Notes = string.IsNullOrWhiteSpace(reason) 
                ? "Rejected by admin" 
                : $"Rejected: {reason}";
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Listing {ListingId} rejected", id);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Listing rejected"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting listing");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPut("{id}/feature")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> FeatureListing(Guid id, [FromBody] bool isFeatured, CancellationToken cancellationToken = default)
    {
        try
        {
            var listing = await _unitOfWork.PropertyListings.GetByIdAsync(id, cancellationToken);
            if (listing == null || listing.IsDeleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Listing not found"));
            }

            listing.IsFeatured = isFeatured;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Listing {ListingId} featured status: {IsFeatured}", id, isFeatured);
            return Ok(ApiResponse<bool>.SuccessResponse(true, $"Listing {(isFeatured ? "featured" : "unfeatured")}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error featuring listing");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteListing(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var listing = await _unitOfWork.PropertyListings.GetByIdAsync(id, cancellationToken);
            if (listing == null || listing.IsDeleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Listing not found"));
            }

            await _unitOfWork.PropertyListings.DeleteAsync(listing);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Listing {ListingId} deleted", id);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Listing deleted"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting listing");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }
}
