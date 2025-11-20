using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.SavedSearch;
using RealEstatePlatform.Application.Interfaces.Repositories;
using RealEstatePlatform.Domain.Entities;
using System.Security.Claims;

namespace RealEstatePlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SavedSearchesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SavedSearchesController> _logger;

    public SavedSearchesController(IUnitOfWork unitOfWork, ILogger<SavedSearchesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<SavedSearchDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSavedSearches(CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<List<SavedSearchDto>>.ErrorResponse("User not authenticated"));
            }

            var searches = await _unitOfWork.SavedSearches.FindAsync(s => s.UserId == userId && !s.IsDeleted, cancellationToken);

            var dtos = searches.OrderByDescending(s => s.CreatedAt).Select(s => new SavedSearchDto
            {
                Id = s.Id,
                Name = s.Name,
                SearchCriteria = s.SearchCriteria,
                IsActive = s.IsActive,
                EmailNotificationEnabled = s.EmailNotificationEnabled,
                NotificationFrequency = s.NotificationFrequency,
                LastNotificationDate = s.LastNotificationDate,
                LastNotificationSent = s.LastNotificationDate,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }).ToList();

            return Ok(ApiResponse<List<SavedSearchDto>>.SuccessResponse(dtos));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving saved searches");
            return StatusCode(500, ApiResponse<List<SavedSearchDto>>.ErrorResponse("An error occurred"));
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SavedSearchDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSavedSearchById(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<SavedSearchDto>.ErrorResponse("User not authenticated"));
            }

            var search = await _unitOfWork.SavedSearches.GetByIdAsync(id, cancellationToken);

            if (search == null || search.IsDeleted || search.UserId != userId)
            {
                return NotFound(ApiResponse<SavedSearchDto>.ErrorResponse("Search not found"));
            }

            var dto = new SavedSearchDto
            {
                Id = search.Id,
                Name = search.Name,
                SearchCriteria = search.SearchCriteria,
                IsActive = search.IsActive,
                EmailNotificationEnabled = search.EmailNotificationEnabled,
                NotificationFrequency = search.NotificationFrequency,
                LastNotificationDate = search.LastNotificationDate,
                LastNotificationSent = search.LastNotificationDate,
                CreatedAt = search.CreatedAt,
                UpdatedAt = search.UpdatedAt
            };

            return Ok(ApiResponse<SavedSearchDto>.SuccessResponse(dto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving saved search");
            return StatusCode(500, ApiResponse<SavedSearchDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SavedSearchDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateSavedSearch([FromBody] CreateSavedSearchDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<SavedSearchDto>.ErrorResponse("User not authenticated"));
            }

            var savedSearch = new SavedSearch
            {
                UserId = userId,
                Name = dto.Name,
                SearchCriteria = dto.SearchCriteria,
                IsActive = dto.IsActive,
                EmailNotificationEnabled = dto.EmailNotificationEnabled,
                NotificationFrequency = dto.NotificationFrequency
            };

            await _unitOfWork.SavedSearches.AddAsync(savedSearch);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var responseDto = new SavedSearchDto
            {
                Id = savedSearch.Id,
                Name = savedSearch.Name,
                SearchCriteria = savedSearch.SearchCriteria,
                IsActive = savedSearch.IsActive,
                EmailNotificationEnabled = savedSearch.EmailNotificationEnabled,
                NotificationFrequency = savedSearch.NotificationFrequency,
                LastNotificationDate = savedSearch.LastNotificationDate,
                LastNotificationSent = savedSearch.LastNotificationDate,
                CreatedAt = savedSearch.CreatedAt,
                UpdatedAt = savedSearch.UpdatedAt
            };

            return CreatedAtAction(nameof(GetSavedSearchById), new { id = savedSearch.Id },
                ApiResponse<SavedSearchDto>.SuccessResponse(responseDto, "Saved search created"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating saved search");
            return StatusCode(500, ApiResponse<SavedSearchDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<SavedSearchDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSavedSearch(Guid id, [FromBody] UpdateSavedSearchDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<SavedSearchDto>.ErrorResponse("User not authenticated"));
            }

            var search = await _unitOfWork.SavedSearches.GetByIdAsync(id, cancellationToken);

            if (search == null || search.IsDeleted || search.UserId != userId)
            {
                return NotFound(ApiResponse<SavedSearchDto>.ErrorResponse("Search not found"));
            }

            if (!string.IsNullOrWhiteSpace(dto.Name))
                search.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.SearchCriteria))
                search.SearchCriteria = dto.SearchCriteria;

            search.IsActive = dto.IsActive;
            search.EmailNotificationEnabled = dto.EmailNotificationEnabled;

            if (!string.IsNullOrWhiteSpace(dto.NotificationFrequency))
                search.NotificationFrequency = dto.NotificationFrequency;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var responseDto = new SavedSearchDto
            {
                Id = search.Id,
                Name = search.Name,
                SearchCriteria = search.SearchCriteria,
                IsActive = search.IsActive,
                EmailNotificationEnabled = search.EmailNotificationEnabled,
                NotificationFrequency = search.NotificationFrequency,
                LastNotificationDate = search.LastNotificationDate,
                LastNotificationSent = search.LastNotificationDate,
                CreatedAt = search.CreatedAt,
                UpdatedAt = search.UpdatedAt
            };

            return Ok(ApiResponse<SavedSearchDto>.SuccessResponse(responseDto, "Saved search updated"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating saved search");
            return StatusCode(500, ApiResponse<SavedSearchDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteSavedSearch(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<bool>.ErrorResponse("User not authenticated"));
            }

            var search = await _unitOfWork.SavedSearches.GetByIdAsync(id, cancellationToken);

            if (search == null || search.IsDeleted || search.UserId != userId)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Search not found"));
            }

            await _unitOfWork.SavedSearches.DeleteAsync(search);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Ok(ApiResponse<bool>.SuccessResponse(true, "Saved search deleted"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting saved search");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPut("{id:guid}/toggle-notifications")]
    [ProducesResponseType(typeof(ApiResponse<SavedSearchDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ToggleNotifications(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<SavedSearchDto>.ErrorResponse("User not authenticated"));
            }

            var search = await _unitOfWork.SavedSearches.GetByIdAsync(id, cancellationToken);

            if (search == null || search.IsDeleted || search.UserId != userId)
            {
                return NotFound(ApiResponse<SavedSearchDto>.ErrorResponse("Search not found"));
            }

            search.EmailNotificationEnabled = !search.EmailNotificationEnabled;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = new SavedSearchDto
            {
                Id = search.Id,
                Name = search.Name,
                SearchCriteria = search.SearchCriteria,
                IsActive = search.IsActive,
                EmailNotificationEnabled = search.EmailNotificationEnabled,
                NotificationFrequency = search.NotificationFrequency,
                LastNotificationDate = search.LastNotificationDate,
                LastNotificationSent = search.LastNotificationDate,
                CreatedAt = search.CreatedAt,
                UpdatedAt = search.UpdatedAt
            };

            return Ok(ApiResponse<SavedSearchDto>.SuccessResponse(dto, $"Notifications {(search.EmailNotificationEnabled ? "enabled" : "disabled")}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling notifications");
            return StatusCode(500, ApiResponse<SavedSearchDto>.ErrorResponse("An error occurred"));
        }
    }
}
