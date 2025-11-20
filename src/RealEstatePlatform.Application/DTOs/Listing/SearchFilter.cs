using RealEstatePlatform.Domain.Enums;

namespace RealEstatePlatform.Application.DTOs.Listing;

/// <summary>
/// Search filter for property listings
/// </summary>
public class SearchFilter
{
    public string? Keyword { get; set; }
    public ListingType? ListingType { get; set; }
    public PropertyType? PropertyType { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinArea { get; set; }
    public decimal? MaxArea { get; set; }
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public Guid? ProvinceId { get; set; }
    public Guid? DistrictId { get; set; }
    public Guid? WardId { get; set; }
    public List<Guid>? AmenityIds { get; set; }
    public bool? IsFeatured { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? RadiusKm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = true;
}
