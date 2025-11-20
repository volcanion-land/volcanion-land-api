using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.Review;
using RealEstatePlatform.Application.Interfaces.Repositories;
using RealEstatePlatform.Application.Interfaces.Services;
using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReviewService> _logger;

    public ReviewService(
        IUnitOfWork unitOfWork,
        ILogger<ReviewService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Guid> CreateReviewAsync(string userId, CreateReviewDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Rating < 1 || dto.Rating > 5)
        {
            throw new ArgumentException("Rating must be between 1 and 5");
        }

        if (dto.ListingId == null && dto.ReviewedUserId == null)
        {
            throw new ArgumentException("Either ListingId or ReviewedUserId must be provided");
        }

        // Validate listing exists if provided
        if (dto.ListingId.HasValue)
        {
            var listingExists = await _unitOfWork.PropertyListings
                .AnyAsync(l => l.Id == dto.ListingId.Value && !l.IsDeleted, cancellationToken);
            
            if (!listingExists)
            {
                throw new InvalidOperationException("Listing not found");
            }
        }

        // Validate reviewed user exists if provided
        if (!string.IsNullOrEmpty(dto.ReviewedUserId))
        {
            var userExists = await _unitOfWork.Users
                .AnyAsync(u => u.Id == dto.ReviewedUserId && !u.IsDeleted, cancellationToken);
            
            if (!userExists)
            {
                throw new InvalidOperationException("Reviewed user not found");
            }
        }

        var review = new Review
        {
            ReviewerId = userId,
            ListingId = dto.ListingId,
            ReviewedUserId = dto.ReviewedUserId,
            Rating = dto.Rating,
            Title = dto.Title,
            Content = dto.Content,
            ParentReviewId = dto.ParentReviewId,
            IsApproved = false // Requires admin approval
        };

        await _unitOfWork.Reviews.AddAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Review created by user {UserId} for listing {ListingId} or user {ReviewedUserId}", 
            userId, dto.ListingId, dto.ReviewedUserId);

        return review.Id;
    }

    public async Task<PaginatedList<ReviewDto>> GetListingReviewsAsync(
        Guid listingId, 
        int pageNumber = 1, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default)
    {
        var allReviews = await _unitOfWork.Reviews.FindAsync(
            r => r.ListingId == listingId && r.IsApproved && !r.IsDeleted && r.ParentReviewId == null,
            cancellationToken);

        var query = allReviews.AsQueryable().OrderByDescending(r => r.CreatedAt);
        var totalCount = query.Count();
        var reviews = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        var dtos = reviews.Select(r => MapToDto(r)).ToList();
        return new PaginatedList<ReviewDto>(dtos, totalCount, pageNumber, pageSize);
    }

    public async Task<PaginatedList<ReviewDto>> GetUserReviewsAsync(
        string userId, 
        int pageNumber = 1, 
        int pageSize = 20, 
        CancellationToken cancellationToken = default)
    {
        var allReviews = await _unitOfWork.Reviews.FindAsync(
            r => r.ReviewedUserId == userId && r.IsApproved && !r.IsDeleted && r.ParentReviewId == null,
            cancellationToken);

        var query = allReviews.AsQueryable().OrderByDescending(r => r.CreatedAt);
        var totalCount = query.Count();
        var reviews = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        var dtos = reviews.Select(r => MapToDto(r)).ToList();
        return new PaginatedList<ReviewDto>(dtos, totalCount, pageNumber, pageSize);
    }

    public async Task<bool> UpdateReviewAsync(Guid reviewId, string userId, UpdateReviewDto dto, CancellationToken cancellationToken = default)
    {
        var review = await _unitOfWork.Reviews
            .FirstOrDefaultAsync(r => r.Id == reviewId && r.ReviewerId == userId && !r.IsDeleted, cancellationToken);

        if (review == null)
        {
            return false;
        }

        if (dto.Rating < 1 || dto.Rating > 5)
        {
            throw new ArgumentException("Rating must be between 1 and 5");
        }

        review.Rating = dto.Rating;
        review.Title = dto.Title;
        review.Content = dto.Content;
        review.IsApproved = false; // Requires re-approval after edit

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Review {ReviewId} updated by user {UserId}", reviewId, userId);

        return true;
    }

    public async Task<bool> DeleteReviewAsync(Guid reviewId, string userId, CancellationToken cancellationToken = default)
    {
        var review = await _unitOfWork.Reviews
            .FirstOrDefaultAsync(r => r.Id == reviewId && r.ReviewerId == userId && !r.IsDeleted, cancellationToken);

        if (review == null)
        {
            return false;
        }

        review.IsDeleted = true;
        review.DeletedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Review {ReviewId} deleted by user {UserId}", reviewId, userId);

        return true;
    }

    public async Task<bool> AdminApproveReviewAsync(Guid reviewId, string? adminResponse, CancellationToken cancellationToken = default)
    {
        var review = await _unitOfWork.Reviews
            .FirstOrDefaultAsync(r => r.Id == reviewId && !r.IsDeleted, cancellationToken);

        if (review == null)
        {
            return false;
        }

        review.IsApproved = true;
        review.AdminResponse = adminResponse;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Review {ReviewId} approved by admin", reviewId);

        return true;
    }

    public async Task<bool> MarkReviewAsHelpfulAsync(Guid reviewId, CancellationToken cancellationToken = default)
    {
        var review = await _unitOfWork.Reviews
            .FirstOrDefaultAsync(r => r.Id == reviewId && !r.IsDeleted, cancellationToken);

        if (review == null)
        {
            return false;
        }

        review.HelpfulCount++;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<double> GetAverageRatingAsync(Guid? listingId, string? userId, CancellationToken cancellationToken = default)
    {
        IEnumerable<Review> allReviews;
        
        if (listingId.HasValue)
        {
            allReviews = await _unitOfWork.Reviews.FindAsync(
                r => r.ListingId == listingId.Value && r.IsApproved && !r.IsDeleted,
                cancellationToken);
        }
        else if (!string.IsNullOrEmpty(userId))
        {
            allReviews = await _unitOfWork.Reviews.FindAsync(
                r => r.ReviewedUserId == userId && r.IsApproved && !r.IsDeleted,
                cancellationToken);
        }
        else
        {
            return 0;
        }

        var reviewsList = allReviews.ToList();
        if (!reviewsList.Any())
        {
            return 0;
        }

        return reviewsList.Average(r => r.Rating);
    }

    private ReviewDto MapToDto(Review review)
    {
        return new ReviewDto
        {
            Id = review.Id,
            ReviewerId = review.ReviewerId,
            ReviewerName = review.Reviewer?.FullName ?? "Unknown",
            ReviewerAvatar = review.Reviewer?.AvatarUrl,
            ListingId = review.ListingId,
            ListingTitle = review.Listing?.Property?.Title,
            ReviewedUserId = review.ReviewedUserId,
            ReviewedUserName = review.ReviewedUser?.FullName,
            Rating = review.Rating,
            Title = review.Title,
            Content = review.Content,
            IsVerified = review.IsVerified,
            IsApproved = review.IsApproved,
            AdminResponse = review.AdminResponse,
            HelpfulCount = review.HelpfulCount,
            ParentReviewId = review.ParentReviewId,
            Replies = review.Replies?.Select(r => MapToDto(r)).ToList(),
            CreatedAt = review.CreatedAt
        };
    }
}
