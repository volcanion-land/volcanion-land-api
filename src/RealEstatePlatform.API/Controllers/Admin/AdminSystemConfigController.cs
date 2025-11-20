using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.Interfaces.Repositories;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.API.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin")]
public class SystemConfigController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SystemConfigController> _logger;

    public SystemConfigController(IUnitOfWork unitOfWork, ILogger<SystemConfigController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<SystemConfigDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllConfigs(CancellationToken cancellationToken = default)
    {
        try
        {
            var configs = await _unitOfWork.SystemConfigurations.GetAllAsync(cancellationToken);
            var dtos = configs.Select(c => new SystemConfigDto
            {
                Id = c.Id,
                Key = c.Key,
                Value = c.Value,
                Description = c.Description,
                DataType = c.DataType,
                UpdatedAt = c.UpdatedAt
            }).ToList();

            return Ok(ApiResponse<List<SystemConfigDto>>.SuccessResponse(dtos));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system configurations");
            return StatusCode(500, ApiResponse<List<SystemConfigDto>>.ErrorResponse("An error occurred"));
        }
    }

    [HttpGet("{key}")]
    [ProducesResponseType(typeof(ApiResponse<SystemConfigDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConfigByKey(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var configs = await _unitOfWork.SystemConfigurations.FindAsync(
                c => c.Key == key,
                cancellationToken);
            
            var config = configs.FirstOrDefault();
            if (config == null)
            {
                return NotFound(ApiResponse<SystemConfigDto>.ErrorResponse("Configuration not found"));
            }

            var dto = new SystemConfigDto
            {
                Id = config.Id,
                Key = config.Key,
                Value = config.Value,
                Description = config.Description,
                DataType = config.DataType,
                UpdatedAt = config.UpdatedAt
            };

            return Ok(ApiResponse<SystemConfigDto>.SuccessResponse(dto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving configuration");
            return StatusCode(500, ApiResponse<SystemConfigDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SystemConfigDto>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateConfig([FromBody] CreateSystemConfigDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var config = new SystemConfiguration
            {
                Key = dto.Key,
                Value = dto.Value,
                Description = dto.Description,
                DataType = dto.DataType
            };

            await _unitOfWork.SystemConfigurations.AddAsync(config);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new SystemConfigDto
            {
                Id = config.Id,
                Key = config.Key,
                Value = config.Value,
                Description = config.Description,
                DataType = config.DataType,
                UpdatedAt = config.UpdatedAt
            };

            _logger.LogInformation("System configuration {Key} created", dto.Key);
            return CreatedAtAction(nameof(GetConfigByKey), new { key = config.Key }, 
                ApiResponse<SystemConfigDto>.SuccessResponse(result, "Configuration created"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating configuration");
            return StatusCode(500, ApiResponse<SystemConfigDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpPut("{key}")]
    [ProducesResponseType(typeof(ApiResponse<SystemConfigDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateConfig(string key, [FromBody] UpdateSystemConfigDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var configs = await _unitOfWork.SystemConfigurations.FindAsync(
                c => c.Key == key,
                cancellationToken);
            
            var config = configs.FirstOrDefault();
            if (config == null)
            {
                return NotFound(ApiResponse<SystemConfigDto>.ErrorResponse("Configuration not found"));
            }

            config.Value = dto.Value;
            config.Description = dto.Description;
            config.DataType = dto.DataType;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new SystemConfigDto
            {
                Id = config.Id,
                Key = config.Key,
                Value = config.Value,
                Description = config.Description,
                DataType = config.DataType,
                UpdatedAt = config.UpdatedAt
            };

            _logger.LogInformation("System configuration {Key} updated", key);
            return Ok(ApiResponse<SystemConfigDto>.SuccessResponse(result, "Configuration updated"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating configuration");
            return StatusCode(500, ApiResponse<SystemConfigDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpDelete("{key}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteConfig(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var configs = await _unitOfWork.SystemConfigurations.FindAsync(
                c => c.Key == key,
                cancellationToken);
            
            var config = configs.FirstOrDefault();
            if (config == null)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse("Configuration not found"));
            }

            await _unitOfWork.SystemConfigurations.DeleteAsync(config);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("System configuration {Key} deleted", key);
            return Ok(ApiResponse<bool>.SuccessResponse(true, "Configuration deleted"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting configuration");
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred"));
        }
    }
}

// DTOs
public class SystemConfigDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DataType { get; set; } = "string";
    public DateTime? UpdatedAt { get; set; }
}

public class CreateSystemConfigDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DataType { get; set; } = "string";
}

public class UpdateSystemConfigDto
{
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DataType { get; set; } = "string";
}
