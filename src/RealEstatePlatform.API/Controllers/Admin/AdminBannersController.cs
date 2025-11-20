using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.Interfaces.Repositories;
using RealEstatePlatform.Domain.Entities;
using RealEstatePlatform.Domain.Enums;

namespace RealEstatePlatform.API.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin,Moderator")]
public class BannersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BannersController> _logger;

    public BannersController(IUnitOfWork unitOfWork, ILogger<BannersController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<BannerDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBanners(
        [FromQuery] string? position,
        [FromQuery] bool? isActive,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var banners = await _unitOfWork.Banners.FindAsync(
                b => !b.IsDeleted && 
                     (string.IsNullOrWhiteSpace(position) || b.Position == position) &&
                     (!isActive.HasValue || b.IsActive == isActive.Value),
                cancellationToken);

            var dtos = banners.Select(b => new BannerDto
            {
                Id = b.Id,
                Title = b.Title,
                ImageUrl = b.ImageUrl,
                LinkUrl = b.LinkUrl,
                Position = b.Position,
                DisplayOrder = b.DisplayOrder,
                IsActive = b.IsActive,
                StartDate = b.StartDate,
                EndDate = b.EndDate,
                ClickCount = b.ClickCount
            }).OrderBy(b => b.DisplayOrder).ToList();

            return Ok(ApiResponse<List<BannerDto>>.SuccessResponse(dtos));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving banners");
            return StatusCode(500, ApiResponse<List<BannerDto>>.ErrorResponse("An error occurred"));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<BannerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBannerById(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var banner = await _unitOfWork.Banners.GetByIdAsync(id, cancellationToken);
            if (banner == null || banner.IsDeleted)
            {
                return NotFound(ApiResponse<BannerDto>.ErrorResponse("Banner not found"));
            }

            var dto = new BannerDto
            {
                Id = banner.Id,
                Title = banner.Title,
                ImageUrl = banner.ImageUrl,
                LinkUrl = banner.LinkUrl,
                Position = banner.Position,
                DisplayOrder = banner.DisplayOrder,
                IsActive = banner.IsActive,
                StartDate = banner.StartDate,
                EndDate = banner.EndDate,
                ClickCount = banner.ClickCount
            };

            return Ok(ApiResponse<BannerDto>.SuccessResponse(dto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving banner");
            return StatusCode(500, ApiResponse<BannerDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<BannerDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateBanner([FromBody] CreateBannerDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var banner = new Banner
            {
                Title = dto.Title,
                ImageUrl = dto.ImageUrl,
                LinkUrl = dto.LinkUrl,
                Position = dto.Position,
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            await _unitOfWork.Banners.AddAsync(banner);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new BannerDto
            {
                Id = banner.Id,
                Title = banner.Title,
                ImageUrl = banner.ImageUrl,
                LinkUrl = banner.LinkUrl,
                Position = banner.Position,
                DisplayOrder = banner.DisplayOrder,
                IsActive = banner.IsActive,
                StartDate = banner.StartDate,
                EndDate = banner.EndDate,
                ClickCount = 0
            };

            _logger.LogInformation("Banner {BannerId} created", banner.Id);
            return CreatedAtAction(nameof(GetBannerById), new { id = banner.Id }, 
                ApiResponse<BannerDto>.SuccessResponse(result, "Banner created"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating banner");
            return StatusCode(500, ApiResponse<BannerDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<BannerDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateBanner(Guid id, [FromBody] UpdateBannerDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var banner = await _unitOfWork.Banners.GetByIdAsync(id, cancellationToken);
            if (banner == null || banner.IsDeleted)
            {
                return NotFound(ApiResponse<BannerDto>.ErrorResponse("Banner not found"));
            }

            banner.Title = dto.Title;
            banner.ImageUrl = dto.ImageUrl;
            banner.LinkUrl = dto.LinkUrl;
            banner.Position = dto.Position;
            banner.DisplayOrder = dto.DisplayOrder;
            banner.IsActive = dto.IsActive;
            banner.StartDate = dto.StartDate;
            banner.EndDate = dto.EndDate;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new BannerDto
            {
                Id = banner.Id,
                Title = banner.Title,
                ImageUrl = banner.ImageUrl,
                LinkUrl = banner.LinkUrl,
                Position = banner.Position,
                DisplayOrder = banner.DisplayOrder,
                IsActive = banner.IsActive,
                StartDate = banner.StartDate,
                EndDate = banner.EndDate,
                ClickCount = banner.ClickCount
            };

            _logger.LogInformation("Banner {BannerId} updated", id);
            return Ok(ApiResponse<BannerDto>.SuccessResponse(result, "Banner updated"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating banner");
            return StatusCode(500, ApiResponse<BannerDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteBanner(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var banner = await _unitOfWork.Banners.GetByIdAsync(id, cancellationToken);
            if (banner == null || banner.IsDeleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Banner not found"));
            }

            await _unitOfWork.Banners.DeleteAsync(banner);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Banner {BannerId} deleted", id);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Banner deleted"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting banner");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }
}

// DTOs
public class BannerDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? LinkUrl { get; set; }
    public string Position { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int ClickCount { get; set; }
}

public class CreateBannerDto
{
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? LinkUrl { get; set; }
    public string Position { get; set; } = "homepage_slider";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class UpdateBannerDto
{
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? LinkUrl { get; set; }
    public string Position { get; set; } = "homepage_slider";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
