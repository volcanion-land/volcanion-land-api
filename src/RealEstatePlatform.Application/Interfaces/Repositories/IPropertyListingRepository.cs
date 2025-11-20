using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.Listing;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Application.Interfaces.Repositories;

/// <summary>
/// Property listing repository interface
/// </summary>
public interface IPropertyListingRepository : IRepository<PropertyListing>
{
    Task<PaginatedList<PropertyListing>> SearchListingsAsync(
        SearchFilter filter, 
        CancellationToken cancellationToken = default);

    Task<PropertyListing?> GetDetailAsync(
        Guid id, 
        CancellationToken cancellationToken = default);

    Task<IEnumerable<PropertyListing>> GetFeaturedListingsAsync(
        int count = 10, 
        CancellationToken cancellationToken = default);

    Task<IEnumerable<PropertyListing>> GetSimilarListingsAsync(
        Guid listingId, 
        int count = 5, 
        CancellationToken cancellationToken = default);

    Task<IEnumerable<PropertyListing>> GetNearbyListingsAsync(
        double latitude, 
        double longitude, 
        double radiusKm, 
        int count = 10, 
        CancellationToken cancellationToken = default);

    Task IncrementViewCountAsync(
        Guid listingId, 
        CancellationToken cancellationToken = default);
}
