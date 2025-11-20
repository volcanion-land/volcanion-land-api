using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RealEstatePlatform.Application.DTOs.Auth;
using RealEstatePlatform.Application.Interfaces.Services;
using RealEstatePlatform.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealEstatePlatform.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IEmailService emailService,
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _emailService = emailService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default)
    {
        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        // Create new user
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create user: {errors}");
        }

        // Assign default role
        await _userManager.AddToRoleAsync(user, "User");

        // Generate email confirmation token
        var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        
        // Send confirmation email
        await _emailService.SendEmailConfirmationAsync(user.Email, user.FullName, emailToken, cancellationToken);

        // Generate JWT token
        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateAccessToken(user.Id, user.Email!, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Save refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("User {Email} registered successfully", dto.Email);

        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(int.Parse(_configuration["JWT:ExpiryInHours"] ?? "24")),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = user.AvatarUrl,
                IsVerified = user.EmailConfirmed,
                Roles = roles.ToList()
            }
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                throw new UnauthorizedAccessException("Account is locked due to multiple failed login attempts");
            }
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Generate tokens
        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateAccessToken(user.Id, user.Email!, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Save refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(dto.RememberMe ? 30 : 7);
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("User {Email} logged in successfully", dto.Email);

        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(int.Parse(_configuration["JWT:ExpiryInHours"] ?? "24")),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = user.AvatarUrl,
                IsVerified = user.EmailConfirmed,
                Roles = roles.ToList()
            }
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);
        
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        // Generate new tokens
        var roles = await _userManager.GetRolesAsync(user);
        var newToken = _tokenService.GenerateAccessToken(user.Id, user.Email!, roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            Token = newToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(int.Parse(_configuration["JWT:ExpiryInHours"] ?? "24")),
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = user.AvatarUrl,
                IsVerified = user.EmailConfirmed,
                Roles = roles.ToList()
            }
        };
    }

    public async Task LogoutAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);
            
            _logger.LogInformation("User {UserId} logged out", userId);
        }
    }

    public async Task<bool> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded)
        {
            _logger.LogInformation("User {Email} confirmed their email", user.Email);
        }

        return result.Succeeded;
    }

    public async Task<bool> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            // Don't reveal that user doesn't exist
            return true;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _emailService.SendPasswordResetAsync(user.Email!, user.FullName, token, cancellationToken);

        _logger.LogInformation("Password reset email sent to {Email}", email);
        return true;
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return false;
        }

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (result.Succeeded)
        {
            _logger.LogInformation("Password reset successful for {Email}", email);
        }

        return result.Succeeded;
    }
}
