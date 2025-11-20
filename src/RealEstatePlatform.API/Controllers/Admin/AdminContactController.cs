using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.Interfaces.Repositories;
using RealEstatePlatform.Application.Interfaces.Services;
using System.Security.Claims;

namespace RealEstatePlatform.API.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin,Moderator")]
public class ContactController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ILogger<ContactController> _logger;

    public ContactController(
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        ILogger<ContactController> logger)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _logger = logger;
    }

    [HttpGet("requests")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedList<AdminContactRequestDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllContactRequests(
        [FromQuery] string? requestType,
        [FromQuery] bool? isResponded,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var requests = await _unitOfWork.ContactRequests.FindAsync(
                r => !r.IsDeleted && 
                     (string.IsNullOrWhiteSpace(requestType) || r.RequestType == requestType) &&
                     (!isResponded.HasValue || r.IsResponded == isResponded.Value),
                cancellationToken);

            var query = requests.AsQueryable().OrderByDescending(r => r.CreatedAt);
            var totalCount = query.Count();
            var pagedRequests = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            var dtos = pagedRequests.Select(r => new AdminContactRequestDto
            {
                Id = r.Id,
                Name = r.Name,
                Email = r.Email,
                Phone = r.Phone,
                Subject = r.Subject,
                Message = r.Message,
                RequestType = r.RequestType,
                IsRead = r.IsRead,
                IsResponded = r.IsResponded,
                AdminResponse = r.AdminResponse,
                ResponseDate = r.ResponseDate,
                RespondedBy = r.RespondedBy,
                CreatedAt = r.CreatedAt,
                ListingId = r.ListingId,
                UserId = r.UserId
            }).ToList();

            var result = new PaginatedList<AdminContactRequestDto>(dtos, totalCount, pageNumber, pageSize);
            return Ok(ApiResponse<PaginatedList<AdminContactRequestDto>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contact requests");
            return StatusCode(500, ApiResponse<PaginatedList<AdminContactRequestDto>>.ErrorResponse("An error occurred"));
        }
    }

    [HttpGet("requests/{id}")]
    [ProducesResponseType(typeof(ApiResponse<AdminContactRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContactRequestById(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = await _unitOfWork.ContactRequests.GetByIdAsync(id, cancellationToken);
            if (request == null || request.IsDeleted)
            {
                return NotFound(ApiResponse<AdminContactRequestDto>.ErrorResponse("Contact request not found"));
            }

            // Mark as read
            if (!request.IsRead)
            {
                request.IsRead = true;
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            var dto = new AdminContactRequestDto
            {
                Id = request.Id,
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Subject = request.Subject,
                Message = request.Message,
                RequestType = request.RequestType,
                IsRead = request.IsRead,
                IsResponded = request.IsResponded,
                AdminResponse = request.AdminResponse,
                ResponseDate = request.ResponseDate,
                RespondedBy = request.RespondedBy,
                CreatedAt = request.CreatedAt,
                ListingId = request.ListingId,
                UserId = request.UserId
            };

            return Ok(ApiResponse<AdminContactRequestDto>.SuccessResponse(dto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contact request");
            return StatusCode(500, ApiResponse<AdminContactRequestDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPost("requests/{id}/respond")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RespondToRequest(
        Guid id,
        [FromBody] RespondToContactDto dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = await _unitOfWork.ContactRequests.GetByIdAsync(id, cancellationToken);
            if (request == null || request.IsDeleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Contact request not found"));
            }

            // Update request
            request.IsResponded = true;
            request.AdminResponse = dto.Response;
            request.ResponseDate = DateTime.UtcNow;
            request.RespondedBy = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            request.IsRead = true;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Send email response to user
            try
            {
                // TODO: Implement SendContactResponseEmailAsync in IEmailService
                // await _emailService.SendContactResponseEmailAsync(request.Email, request.Name, dto.Response);
                _logger.LogInformation("Email response would be sent to {Email}", request.Email);
            }
            catch (Exception emailEx)
            {
                _logger.LogError(emailEx, "Error sending response email to {Email}", request.Email);
                // Continue even if email fails
            }

            _logger.LogInformation("Responded to contact request {RequestId}", id);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Response sent successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error responding to contact request");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPut("requests/{id}/read")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = await _unitOfWork.ContactRequests.GetByIdAsync(id, cancellationToken);
            if (request == null || request.IsDeleted)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Contact request not found"));
            }

            request.IsRead = true;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Contact request {RequestId} marked as read", id);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Marked as read"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking contact request as read");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }
}

// DTOs
public class AdminContactRequestDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Subject { get; set; }
    public string Message { get; set; } = string.Empty;
    public string RequestType { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public bool IsResponded { get; set; }
    public string? AdminResponse { get; set; }
    public DateTime? ResponseDate { get; set; }
    public string? RespondedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? ListingId { get; set; }
    public string? UserId { get; set; }
}

public class RespondToContactDto
{
    public string Response { get; set; } = string.Empty;
}
