using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.Auth;
using RealEstatePlatform.Application.Interfaces.Services;

namespace RealEstatePlatform.API.Controllers;

/// <summary>
/// Authentication controller
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto, cancellationToken);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Registration successful"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration");
            return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResponse("An error occurred during registration"));
        }
    }

    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.LoginAsync(dto, cancellationToken);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Login successful"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResponse("An error occurred during login"));
        }
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(refreshToken, cancellationToken);
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResponse("An error occurred while refreshing token"));
        }
    }

    /// <summary>
    /// Logout user
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value;
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await _authService.LogoutAsync(userId, cancellationToken);
            return Ok(ApiResponse<string>.SuccessResponse("Logout successful"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, ApiResponse<string>.ErrorResponse("An error occurred during logout"));
        }
    }
}
