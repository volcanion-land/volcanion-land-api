using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.Contact;
using RealEstatePlatform.Application.Interfaces.Repositories;
using RealEstatePlatform.Application.Interfaces.Services;
using RealEstatePlatform.Domain.Entities;
using System.Security.Claims;

namespace RealEstatePlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ILogger<ContactController> _logger;

    public ContactController(IUnitOfWork unitOfWork, IEmailService emailService, ILogger<ContactController> logger)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ContactRequestDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> SubmitContactRequest([FromBody] CreateContactRequestDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");

            var contactRequest = new ContactRequest
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone ?? dto.PhoneNumber,
                Subject = dto.Subject,
                Message = dto.Message,
                RequestType = dto.RequestType,
                ListingId = dto.ListingId ?? dto.RelatedListingId,
                UserId = userId,
                IsRead = false,
                IsResponded = false
            };

            await _unitOfWork.ContactRequests.AddAsync(contactRequest);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            try
            {
                await _emailService.SendContactRequestNotificationAsync("admin@realestate.com", dto.Name, dto.Subject ?? "New Contact Request", dto.Message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send notification email");
            }

            var responseDto = new ContactRequestDto
            {
                Id = contactRequest.Id,
                Name = contactRequest.Name,
                Email = contactRequest.Email,
                Phone = contactRequest.Phone,
                Subject = contactRequest.Subject,
                Message = contactRequest.Message,
                RequestType = contactRequest.RequestType,
                ListingId = contactRequest.ListingId,
                RelatedListingId = contactRequest.ListingId,
                IsRead = contactRequest.IsRead,
                IsResponded = contactRequest.IsResponded,
                CreatedAt = contactRequest.CreatedAt
            };

            return CreatedAtAction(nameof(GetContactRequestById), new { id = contactRequest.Id },
                ApiResponse<ContactRequestDto>.SuccessResponse(responseDto, "Request submitted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting contact request");
            return StatusCode(500, ApiResponse<ContactRequestDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpGet("my-requests")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<List<ContactRequestDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserContactRequests(CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<List<ContactRequestDto>>.ErrorResponse("User not authenticated"));
            }

            var requests = await _unitOfWork.ContactRequests.FindAsync(r => r.UserId == userId && !r.IsDeleted, cancellationToken);

            var dtos = requests.OrderByDescending(r => r.CreatedAt).Select(r => new ContactRequestDto
            {
                Id = r.Id,
                Name = r.Name,
                Email = r.Email,
                Phone = r.Phone,
                Subject = r.Subject,
                Message = r.Message,
                RequestType = r.RequestType,
                ListingId = r.ListingId,
                RelatedListingId = r.ListingId,
                IsRead = r.IsRead,
                IsResponded = r.IsResponded,
                AdminResponse = r.AdminResponse,
                ResponseDate = r.ResponseDate,
                RespondedAt = r.ResponseDate,
                RespondedBy = r.RespondedBy,
                CreatedAt = r.CreatedAt
            }).ToList();

            return Ok(ApiResponse<List<ContactRequestDto>>.SuccessResponse(dtos));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contact requests");
            return StatusCode(500, ApiResponse<List<ContactRequestDto>>.ErrorResponse("An error occurred"));
        }
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<ContactRequestDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContactRequestById(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(ApiResponse<ContactRequestDto>.ErrorResponse("User not authenticated"));
            }

            var request = await _unitOfWork.ContactRequests.GetByIdAsync(id, cancellationToken);

            if (request == null || request.IsDeleted)
            {
                return NotFound(ApiResponse<ContactRequestDto>.ErrorResponse("Request not found"));
            }

            if (request.UserId != userId && !User.IsInRole("Admin") && !User.IsInRole("Moderator"))
            {
                return StatusCode(403, ApiResponse<ContactRequestDto>.ErrorResponse("Access denied"));
            }

            var dto = new ContactRequestDto
            {
                Id = request.Id,
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                Subject = request.Subject,
                Message = request.Message,
                RequestType = request.RequestType,
                ListingId = request.ListingId,
                RelatedListingId = request.ListingId,
                IsRead = request.IsRead,
                IsResponded = request.IsResponded,
                AdminResponse = request.AdminResponse,
                ResponseDate = request.ResponseDate,
                RespondedAt = request.ResponseDate,
                RespondedBy = request.RespondedBy,
                CreatedAt = request.CreatedAt
            };

            return Ok(ApiResponse<ContactRequestDto>.SuccessResponse(dto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contact request");
            return StatusCode(500, ApiResponse<ContactRequestDto>.ErrorResponse("An error occurred"));
        }
    }
}
