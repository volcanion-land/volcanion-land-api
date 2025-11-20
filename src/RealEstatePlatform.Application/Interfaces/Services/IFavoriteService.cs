using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.Favorite;

namespace RealEstatePlatform.Application.Interfaces.Services;

/// <summary>
/// Favorite service interface
/// </summary>
public interface IFavoriteService
{
    Task<bool> AddToFavoritesAsync(string userId, Guid listingId, string? note = null, CancellationToken cancellationToken = default);
    Task<bool> RemoveFromFavoritesAsync(string userId, Guid listingId, CancellationToken cancellationToken = default);
    Task<PaginatedList<FavoriteDto>> GetUserFavoritesAsync(string userId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<bool> IsFavoriteAsync(string userId, Guid listingId, CancellationToken cancellationToken = default);
    Task<bool> IsListingFavoritedAsync(string userId, Guid listingId, CancellationToken cancellationToken = default);
    Task<int> GetFavoriteCountAsync(string userId, CancellationToken cancellationToken = default);
}
