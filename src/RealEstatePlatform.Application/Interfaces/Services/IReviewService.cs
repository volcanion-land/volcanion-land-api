using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.Review;

namespace RealEstatePlatform.Application.Interfaces.Services;

public interface IReviewService
{
    Task<Guid> CreateReviewAsync(string userId, CreateReviewDto dto, CancellationToken cancellationToken = default);
    Task<PaginatedList<ReviewDto>> GetListingReviewsAsync(Guid listingId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<PaginatedList<ReviewDto>> GetUserReviewsAsync(string userId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);
    Task<bool> UpdateReviewAsync(Guid reviewId, string userId, UpdateReviewDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteReviewAsync(Guid reviewId, string userId, CancellationToken cancellationToken = default);
    Task<bool> AdminApproveReviewAsync(Guid reviewId, string? adminResponse, CancellationToken cancellationToken = default);
    Task<bool> MarkReviewAsHelpfulAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<double> GetAverageRatingAsync(Guid? listingId, string? userId, CancellationToken cancellationToken = default);
}
