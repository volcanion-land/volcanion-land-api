using RealEstatePlatform.Domain.Enums;

namespace RealEstatePlatform.Application.DTOs.Listing;

/// <summary>
/// Property listing DTO for list views
/// </summary>
public class ListingDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public PropertyType PropertyType { get; set; }
    public ListingType ListingType { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal Area { get; set; }
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public bool IsFeatured { get; set; }
    public ListingStatus Status { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string WardName { get; set; } = string.Empty;
    public string DistrictName { get; set; } = string.Empty;
    public string ProvinceName { get; set; } = string.Empty;
}
