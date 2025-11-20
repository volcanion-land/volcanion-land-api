using System.ComponentModel.DataAnnotations;

namespace RealEstatePlatform.Application.DTOs.User;

/// <summary>
/// DTO for updating user profile
/// </summary>
public class UpdateProfileDto
{
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Phone]
    public string? PhoneNumber { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }

    [MaxLength(1000)]
    public string? Bio { get; set; }

    public string? AvatarUrl { get; set; }
}
