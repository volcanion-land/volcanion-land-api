using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.Interfaces.Repositories;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.API.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin,Moderator")]
public class FAQsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FAQsController> _logger;

    public FAQsController(IUnitOfWork unitOfWork, ILogger<FAQsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<FAQDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllFAQs(
        [FromQuery] string? category,
        [FromQuery] bool? isPublished,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var faqs = await _unitOfWork.FAQs.FindAsync(
                f => !f.IsDeleted && 
                     (string.IsNullOrWhiteSpace(category) || f.Category == category) &&
                     (!isPublished.HasValue || f.IsPublished == isPublished.Value),
                cancellationToken);

            var dtos = faqs.Select(f => new FAQDto
            {
                Id = f.Id,
                Question = f.Question,
                Answer = f.Answer,
                Category = f.Category ?? "general",
                DisplayOrder = f.DisplayOrder,
                IsPublished = f.IsPublished,
                ViewCount = f.ViewCount,
                HelpfulCount = f.HelpfulCount,
                NotHelpfulCount = f.NotHelpfulCount,
                CreatedAt = f.CreatedAt
            }).OrderBy(f => f.DisplayOrder).ToList();

            return Ok(ApiResponse<List<FAQDto>>.SuccessResponse(dtos));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving FAQs");
            return StatusCode(500, ApiResponse<List<FAQDto>>.ErrorResponse("An error occurred"));
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<FAQDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFAQById(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var faq = await _unitOfWork.FAQs.GetByIdAsync(id, cancellationToken);
            if (faq == null || faq.IsDeleted)
            {
                return NotFound(ApiResponse<FAQDto>.ErrorResponse("FAQ not found"));
            }

            var dto = new FAQDto
            {
                Id = faq.Id,
                Question = faq.Question,
                Answer = faq.Answer,
                Category = faq.Category ?? "general",
                DisplayOrder = faq.DisplayOrder,
                IsPublished = faq.IsPublished,
                ViewCount = faq.ViewCount,
                HelpfulCount = faq.HelpfulCount,
                NotHelpfulCount = faq.NotHelpfulCount,
                CreatedAt = faq.CreatedAt
            };

            return Ok(ApiResponse<FAQDto>.SuccessResponse(dto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving FAQ");
            return StatusCode(500, ApiResponse<FAQDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<FAQDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateFAQ([FromBody] CreateFAQDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var faq = new FAQ
            {
                Question = dto.Question,
                Answer = dto.Answer,
                Category = dto.Category,
                DisplayOrder = dto.DisplayOrder,
                IsPublished = dto.IsPublished
            };

            await _unitOfWork.FAQs.AddAsync(faq);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new FAQDto
            {
                Id = faq.Id,
                Question = faq.Question,
                Answer = faq.Answer,
                Category = faq.Category,
                DisplayOrder = faq.DisplayOrder,
                IsPublished = faq.IsPublished,
                ViewCount = 0,
                HelpfulCount = 0,
                NotHelpfulCount = 0,
                CreatedAt = faq.CreatedAt
            };

            _logger.LogInformation("FAQ {FAQId} created", faq.Id);
            return CreatedAtAction(nameof(GetFAQById), new { id = faq.Id }, 
                ApiResponse<FAQDto>.SuccessResponse(result, "FAQ created"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating FAQ");
            return StatusCode(500, ApiResponse<FAQDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<FAQDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateFAQ(Guid id, [FromBody] UpdateFAQDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var faq = await _unitOfWork.FAQs.GetByIdAsync(id, cancellationToken);
            if (faq == null || faq.IsDeleted)
            {
                return NotFound(ApiResponse<FAQDto>.ErrorResponse("FAQ not found"));
            }

            faq.Question = dto.Question;
            faq.Answer = dto.Answer;
            faq.Category = dto.Category;
            faq.DisplayOrder = dto.DisplayOrder;
            faq.IsPublished = dto.IsPublished;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new FAQDto
            {
                Id = faq.Id,
                Question = faq.Question,
                Answer = faq.Answer,
                Category = faq.Category,
                DisplayOrder = faq.DisplayOrder,
                IsPublished = faq.IsPublished,
                ViewCount = faq.ViewCount,
                HelpfulCount = faq.HelpfulCount,
                NotHelpfulCount = faq.NotHelpfulCount,
                CreatedAt = faq.CreatedAt
            };

            _logger.LogInformation("FAQ {FAQId} updated", id);
            return Ok(ApiResponse<FAQDto>.SuccessResponse(result, "FAQ updated"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating FAQ");
            return StatusCode(500, ApiResponse<FAQDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteFAQ(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var faq = await _unitOfWork.FAQs.GetByIdAsync(id, cancellationToken);
            if (faq == null || faq.IsDeleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("FAQ not found"));
            }

            await _unitOfWork.FAQs.DeleteAsync(faq);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("FAQ {FAQId} deleted", id);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "FAQ deleted"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting FAQ");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }
}

// DTOs
public class FAQDto
{
    public Guid Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; }
    public int ViewCount { get; set; }
    public int HelpfulCount { get; set; }
    public int NotHelpfulCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateFAQDto
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string Category { get; set; } = "general";
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; } = true;
}

public class UpdateFAQDto
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string Category { get; set; } = "general";
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; }
}
