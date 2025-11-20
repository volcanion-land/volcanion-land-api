using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.Listing;
using RealEstatePlatform.Domain.Enums;

namespace RealEstatePlatform.Application.Interfaces.Services;

/// <summary>
/// Property listing service interface
/// </summary>
public interface IPropertyListingService
{
    Task<PaginatedList<ListingDto>> SearchListingsAsync(SearchFilter filter, CancellationToken cancellationToken = default);
    Task<ListingDetailDto?> GetListingDetailAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid> CreateListingAsync(CreateListingDto dto, string userId, CancellationToken cancellationToken = default);
    Task UpdateListingAsync(Guid id, CreateListingDto dto, string userId, CancellationToken cancellationToken = default);
    Task UpdateListingStatusAsync(Guid listingId, ListingStatus status, CancellationToken cancellationToken = default);
    Task DeleteListingAsync(Guid id, string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ListingDto>> GetUserListingsAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ListingDto>> GetFeaturedListingsAsync(int count = 10, CancellationToken cancellationToken = default);
}
