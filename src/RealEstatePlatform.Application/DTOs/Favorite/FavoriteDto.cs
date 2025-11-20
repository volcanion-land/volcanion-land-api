using RealEstatePlatform.Domain.Enums;

namespace RealEstatePlatform.Application.DTOs.Favorite;

/// <summary>
/// Favorite listing DTO
/// </summary>
public class FavoriteDto
{
    public Guid Id { get; set; }
    public Guid PropertyListingId { get; set; }
    public Guid ListingId { get; set; }
    public string ListingTitle { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Area { get; set; }
    public string Address { get; set; } = string.Empty;
    public PropertyType PropertyType { get; set; }
    public ListingType ListingType { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public string? Location { get; set; }
    public string? Note { get; set; }
    public DateTime AddedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
