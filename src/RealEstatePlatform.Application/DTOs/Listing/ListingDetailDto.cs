using RealEstatePlatform.Domain.Enums;

namespace RealEstatePlatform.Application.DTOs.Listing;

/// <summary>
/// Detailed property listing DTO
/// </summary>
public class ListingDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public PropertyType PropertyType { get; set; }
    public ListingType ListingType { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal Area { get; set; }
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public int? Floors { get; set; }
    public int? YearBuilt { get; set; }
    public bool IsFeatured { get; set; }
    public ListingStatus Status { get; set; }
    public int ViewCount { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string? ContactEmail { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<PropertyImageDto> Images { get; set; } = new();
    public List<AmenityDto> Amenities { get; set; } = new();
    public LocationDto Location { get; set; } = new();
    public UserSummaryDto Owner { get; set; } = new();
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class PropertyImageDto
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
}

public class AmenityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
}

public class LocationDto
{
    public Guid WardId { get; set; }
    public string WardName { get; set; } = string.Empty;
    public Guid DistrictId { get; set; }
    public string DistrictName { get; set; } = string.Empty;
    public Guid ProvinceId { get; set; }
    public string ProvinceName { get; set; } = string.Empty;
}

public class UserSummaryDto
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
}
