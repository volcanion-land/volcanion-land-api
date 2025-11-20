using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.Favorite;
using RealEstatePlatform.Application.DTOs.Listing;
using RealEstatePlatform.Application.Interfaces.Repositories;
using RealEstatePlatform.Application.Interfaces.Services;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Application.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FavoriteService> _logger;

    public FavoriteService(
        IUnitOfWork unitOfWork,
        ILogger<FavoriteService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> AddToFavoritesAsync(string userId, Guid listingId, string? note = null, CancellationToken cancellationToken = default)
    {
        // Check if listing exists
        var listingExists = await _unitOfWork.PropertyListings
            .AnyAsync(l => l.Id == listingId && !l.IsDeleted, cancellationToken);

        if (!listingExists)
        {
            throw new InvalidOperationException("Listing not found");
        }

        // Check if already favorited
        var existing = await _unitOfWork.FavoriteListings
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ListingId == listingId, cancellationToken);

        if (existing != null)
        {
            if (existing.IsDeleted)
            {
                // Restore deleted favorite
                existing.IsDeleted = false;
                existing.DeletedAt = null;
                existing.Note = note;
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Favorite restored for user {UserId}, listing {ListingId}", userId, listingId);
                return true;
            }
            return false; // Already favorited
        }

        var favorite = new FavoriteListing
        {
            UserId = userId,
            ListingId = listingId,
            Note = note
        };

        await _unitOfWork.FavoriteListings.AddAsync(favorite, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Listing {ListingId} added to favorites by user {UserId}", listingId, userId);
        return true;
    }

    public async Task<bool> RemoveFromFavoritesAsync(string userId, Guid listingId, CancellationToken cancellationToken = default)
    {
        var favorite = await _unitOfWork.FavoriteListings
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ListingId == listingId && !f.IsDeleted, cancellationToken);

        if (favorite == null)
        {
            return false;
        }

        favorite.IsDeleted = true;
        favorite.DeletedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Listing {ListingId} removed from favorites by user {UserId}", listingId, userId);

        return true;
    }

    public async Task<PaginatedList<FavoriteDto>> GetUserFavoritesAsync(
        string userId, 
        int pageNumber = 1, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default)
    {
        var allFavorites = await _unitOfWork.FavoriteListings.FindAsync(
            f => f.UserId == userId && !f.IsDeleted,
            cancellationToken);

        var query = allFavorites.AsQueryable().OrderByDescending(f => f.CreatedAt);
        var totalCount = query.Count();
        var favorites = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        var dtos = favorites
            .Where(f => f.Listing != null && f.Listing.Property != null)
            .Select(f => new FavoriteDto
            {
                Id = f.Id,
                ListingId = f.ListingId,
                ListingTitle = f.Listing!.Property!.Title,
                Price = f.Listing!.Price,
                Area = f.Listing.Property!.Area,
                Address = f.Listing.Property.Address,
                PropertyType = f.Listing.Property.PropertyType,
                ListingType = f.Listing.ListingType,
                PrimaryImageUrl = f.Listing.Property.Images
                    .Where(img => img.IsPrimary)
                    .Select(img => img.ImageUrl)
                    .FirstOrDefault() ?? f.Listing.Property.Images
                    .OrderBy(img => img.DisplayOrder)
                    .Select(img => img.ImageUrl)
                    .FirstOrDefault(),
                Location = f.Listing.Property.Ward != null ? 
                    $"{f.Listing.Property.Ward.Name}, {f.Listing.Property.Ward.District?.Name}, {f.Listing.Property.Ward.District?.Province?.Name}" : 
                    null,
                AddedAt = f.CreatedAt
            }).ToList();

        return new PaginatedList<FavoriteDto>(dtos, totalCount, pageNumber, pageSize);
    }

    public async Task<bool> IsListingFavoritedAsync(string userId, Guid listingId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.FavoriteListings
            .AnyAsync(f => f.UserId == userId && f.ListingId == listingId && !f.IsDeleted, cancellationToken);
    }

    public async Task<bool> IsFavoriteAsync(string userId, Guid listingId, CancellationToken cancellationToken = default)
    {
        return await IsListingFavoritedAsync(userId, listingId, cancellationToken);
    }

    public async Task<int> GetFavoriteCountAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.FavoriteListings
            .CountAsync(f => f.UserId == userId && !f.IsDeleted, cancellationToken);
    }
}
