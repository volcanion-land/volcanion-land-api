using RealEstatePlatform.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace RealEstatePlatform.Application.DTOs.Listing;

/// <summary>
/// DTO for creating a new listing
/// </summary>
public class CreateListingDto
{
    [Required]
    [MaxLength(500)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(5000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public PropertyType PropertyType { get; set; }

    [Required]
    public ListingType ListingType { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Area { get; set; }

    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public int? Floors { get; set; }
    public int? YearBuilt { get; set; }

    [Required]
    [MaxLength(500)]
    public string Address { get; set; } = string.Empty;

    [Required]
    public Guid WardId { get; set; }

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    [Required]
    [MaxLength(200)]
    public string ContactName { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string ContactPhone { get; set; } = string.Empty;

    [EmailAddress]
    public string? ContactEmail { get; set; }

    public List<string> ImageUrls { get; set; } = new();
    public List<Guid> AmenityIds { get; set; } = new();
    public Guid? ListingPackageId { get; set; }
}
