using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.Listing;
using RealEstatePlatform.Application.Interfaces.Repositories;
using RealEstatePlatform.Application.Interfaces.Services;
using RealEstatePlatform.Domain.Entities;
using RealEstatePlatform.Domain.Enums;

namespace RealEstatePlatform.Application.Services;

/// <summary>
/// Property listing service implementation
/// </summary>
public class PropertyListingService : IPropertyListingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICacheService _cacheService;

    public PropertyListingService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _cacheService = cacheService;
    }

    public async Task<PaginatedList<ListingDto>> SearchListingsAsync(
        SearchFilter filter,
        CancellationToken cancellationToken = default)
    {
        // Try to get from cache
        var cacheKey = $"listings_search_{GenerateCacheKey(filter)}";
        var cachedResult = await _cacheService.GetAsync<PaginatedList<ListingDto>>(cacheKey, cancellationToken);
        
        if (cachedResult != null)
            return cachedResult;

        var paginatedListings = await _unitOfWork.PropertyListings.SearchListingsAsync(filter, cancellationToken);

        var listingDtos = paginatedListings.Items.Select(pl => new ListingDto
        {
            Id = pl.Id,
            Title = pl.Property.Title,
            Address = pl.Property.Address,
            PropertyType = pl.Property.PropertyType,
            ListingType = pl.ListingType,
            Price = pl.Price,
            Currency = pl.Currency,
            Area = pl.Property.Area,
            Bedrooms = pl.Property.Bedrooms,
            Bathrooms = pl.Property.Bathrooms,
            PrimaryImageUrl = pl.Property.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl
                ?? pl.Property.Images.FirstOrDefault()?.ImageUrl,
            IsFeatured = pl.IsFeatured,
            Status = pl.Status,
            ViewCount = pl.ViewCount,
            CreatedAt = pl.CreatedAt,
            WardName = pl.Property.Ward.Name,
            DistrictName = pl.Property.Ward.District.Name,
            ProvinceName = pl.Property.Ward.District.Province.Name
        }).ToList();

        var result = new PaginatedList<ListingDto>(
            listingDtos,
            paginatedListings.TotalCount,
            paginatedListings.PageNumber,
            paginatedListings.PageSize);

        // Cache the result for 5 minutes
        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5), cancellationToken);

        return result;
    }

    public async Task<ListingDetailDto?> GetListingDetailAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var listing = await _unitOfWork.PropertyListings.GetDetailAsync(id, cancellationToken);
        
        if (listing == null)
            return null;

        // Increment view count asynchronously
        _ = _unitOfWork.PropertyListings.IncrementViewCountAsync(id, cancellationToken);

        var dto = new ListingDetailDto
        {
            Id = listing.Id,
            Title = listing.Property.Title,
            Description = listing.Property.Description,
            Address = listing.Property.Address,
            PropertyType = listing.Property.PropertyType,
            ListingType = listing.ListingType,
            Price = listing.Price,
            Currency = listing.Currency,
            Area = listing.Property.Area,
            Bedrooms = listing.Property.Bedrooms,
            Bathrooms = listing.Property.Bathrooms,
            Floors = listing.Property.Floors,
            YearBuilt = listing.Property.YearBuilt,
            IsFeatured = listing.IsFeatured,
            Status = listing.Status,
            ViewCount = listing.ViewCount,
            ContactName = listing.ContactName,
            ContactPhone = listing.ContactPhone,
            ContactEmail = listing.ContactEmail,
            CreatedAt = listing.CreatedAt,
            UpdatedAt = listing.UpdatedAt,
            Images = listing.Property.Images.Select(i => new PropertyImageDto
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl,
                Caption = i.Caption,
                IsPrimary = i.IsPrimary,
                DisplayOrder = i.DisplayOrder
            }).ToList(),
            Amenities = listing.Property.PropertyAmenities.Select(pa => new AmenityDto
            {
                Id = pa.Amenity.Id,
                Name = pa.Amenity.Name,
                IconUrl = pa.Amenity.IconUrl
            }).ToList(),
            Location = new LocationDto
            {
                WardId = listing.Property.Ward.Id,
                WardName = listing.Property.Ward.Name,
                DistrictId = listing.Property.Ward.District.Id,
                DistrictName = listing.Property.Ward.District.Name,
                ProvinceId = listing.Property.Ward.District.Province.Id,
                ProvinceName = listing.Property.Ward.District.Province.Name
            },
            Owner = new UserSummaryDto
            {
                Id = listing.Property.Owner.Id,
                FullName = listing.Property.Owner.FullName,
                AvatarUrl = listing.Property.Owner.AvatarUrl,
                PhoneNumber = listing.Property.Owner.PhoneNumber,
                Email = listing.Property.Owner.Email
            },
            Latitude = listing.Property.Location?.Y,
            Longitude = listing.Property.Location?.X
        };

        return dto;
    }

    public async Task<Guid> CreateListingAsync(
        CreateListingDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        Point? location = null;
        if (dto.Latitude.HasValue && dto.Longitude.HasValue)
        {
            location = new Point(dto.Longitude.Value, dto.Latitude.Value) { SRID = 4326 };
        }

        var property = new Property
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            PropertyType = dto.PropertyType,
            Area = dto.Area,
            Bedrooms = dto.Bedrooms,
            Bathrooms = dto.Bathrooms,
            Floors = dto.Floors,
            YearBuilt = dto.YearBuilt,
            Address = dto.Address,
            WardId = dto.WardId,
            Location = location,
            OwnerId = userId
        };

        // Add images
        if (dto.ImageUrls.Any())
        {
            for (int i = 0; i < dto.ImageUrls.Count; i++)
            {
                property.Images.Add(new PropertyImage
                {
                    Id = Guid.NewGuid(),
                    ImageUrl = dto.ImageUrls[i],
                    DisplayOrder = i,
                    IsPrimary = i == 0,
                    PropertyId = property.Id
                });
            }
        }

        // Add amenities
        if (dto.AmenityIds.Any())
        {
            foreach (var amenityId in dto.AmenityIds)
            {
                property.PropertyAmenities.Add(new PropertyAmenity
                {
                    Id = Guid.NewGuid(),
                    PropertyId = property.Id,
                    AmenityId = amenityId
                });
            }
        }

        await _unitOfWork.Repository<Property>().AddAsync(property, cancellationToken);

        var listing = new PropertyListing
        {
            Id = Guid.NewGuid(),
            ListingType = dto.ListingType,
            Price = dto.Price,
            Status = ListingStatus.Active,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30),
            ContactName = dto.ContactName,
            ContactPhone = dto.ContactPhone,
            ContactEmail = dto.ContactEmail,
            PropertyId = property.Id,
            UserId = userId,
            ListingPackageId = dto.ListingPackageId
        };

        await _unitOfWork.PropertyListings.AddAsync(listing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        await InvalidateListingCache();

        return listing.Id;
    }

    public async Task UpdateListingAsync(
        Guid id,
        CreateListingDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var listing = await _unitOfWork.PropertyListings.GetDetailAsync(id, cancellationToken);
        
        if (listing == null || listing.UserId != userId)
            throw new UnauthorizedAccessException("You don't have permission to update this listing");

        // Update property details
        listing.Property.Title = dto.Title;
        listing.Property.Description = dto.Description;
        listing.Property.PropertyType = dto.PropertyType;
        listing.Property.Area = dto.Area;
        listing.Property.Bedrooms = dto.Bedrooms;
        listing.Property.Bathrooms = dto.Bathrooms;
        listing.Property.Floors = dto.Floors;
        listing.Property.YearBuilt = dto.YearBuilt;
        listing.Property.Address = dto.Address;
        listing.Property.WardId = dto.WardId;

        if (dto.Latitude.HasValue && dto.Longitude.HasValue)
        {
            listing.Property.Location = new Point(dto.Longitude.Value, dto.Latitude.Value) { SRID = 4326 };
        }

        // Update listing details
        listing.ListingType = dto.ListingType;
        listing.Price = dto.Price;
        listing.ContactName = dto.ContactName;
        listing.ContactPhone = dto.ContactPhone;
        listing.ContactEmail = dto.ContactEmail;

        await _unitOfWork.PropertyListings.UpdateAsync(listing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        await InvalidateListingCache();
    }

    public async Task UpdateListingStatusAsync(
        Guid listingId,
        ListingStatus status,
        CancellationToken cancellationToken = default)
    {
        var listing = await _unitOfWork.PropertyListings.GetByIdAsync(listingId, cancellationToken);
        
        if (listing == null)
            throw new KeyNotFoundException("Listing not found");

        listing.Status = status;
        await _unitOfWork.PropertyListings.UpdateAsync(listing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        await InvalidateListingCache();
    }

    public async Task DeleteListingAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var listing = await _unitOfWork.PropertyListings.GetByIdAsync(id, cancellationToken);
        
        if (listing == null || listing.UserId != userId)
            throw new UnauthorizedAccessException("You don't have permission to delete this listing");

        listing.IsDeleted = true;
        listing.DeletedAt = DateTime.UtcNow;

        await _unitOfWork.PropertyListings.UpdateAsync(listing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Invalidate cache
        await InvalidateListingCache();
    }

    public async Task<IEnumerable<ListingDto>> GetUserListingsAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var listings = await _unitOfWork.PropertyListings.FindAsync(
            pl => pl.UserId == userId,
            cancellationToken);

        return listings.Select(pl => new ListingDto
        {
            Id = pl.Id,
            Title = pl.Property.Title,
            Address = pl.Property.Address,
            PropertyType = pl.Property.PropertyType,
            ListingType = pl.ListingType,
            Price = pl.Price,
            Currency = pl.Currency,
            Area = pl.Property.Area,
            Status = pl.Status,
            ViewCount = pl.ViewCount,
            CreatedAt = pl.CreatedAt
        });
    }

    public async Task<IEnumerable<ListingDto>> GetFeaturedListingsAsync(
        int count = 10,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"featured_listings_{count}";
        var cachedResult = await _cacheService.GetAsync<IEnumerable<ListingDto>>(cacheKey, cancellationToken);
        
        if (cachedResult != null)
            return cachedResult;

        var listings = await _unitOfWork.PropertyListings.GetFeaturedListingsAsync(count, cancellationToken);

        var result = listings.Select(pl => new ListingDto
        {
            Id = pl.Id,
            Title = pl.Property.Title,
            Address = pl.Property.Address,
            PropertyType = pl.Property.PropertyType,
            ListingType = pl.ListingType,
            Price = pl.Price,
            Currency = pl.Currency,
            Area = pl.Property.Area,
            Bedrooms = pl.Property.Bedrooms,
            Bathrooms = pl.Property.Bathrooms,
            PrimaryImageUrl = pl.Property.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl,
            IsFeatured = pl.IsFeatured,
            Status = pl.Status,
            ViewCount = pl.ViewCount,
            CreatedAt = pl.CreatedAt,
            WardName = pl.Property.Ward.Name,
            DistrictName = pl.Property.Ward.District.Name,
            ProvinceName = pl.Property.Ward.District.Province.Name
        }).ToList();

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(15), cancellationToken);

        return result;
    }

    private string GenerateCacheKey(SearchFilter filter)
    {
        return $"{filter.Keyword}_{filter.ListingType}_{filter.PropertyType}_{filter.MinPrice}_{filter.MaxPrice}_" +
               $"{filter.MinArea}_{filter.MaxArea}_{filter.Bedrooms}_{filter.Bathrooms}_{filter.ProvinceId}_" +
               $"{filter.DistrictId}_{filter.WardId}_{filter.IsFeatured}_{filter.PageNumber}_{filter.PageSize}_" +
               $"{filter.SortBy}_{filter.SortDescending}";
    }

    private async Task InvalidateListingCache()
    {
        await _cacheService.RemoveByPrefixAsync("listings_search_");
        await _cacheService.RemoveByPrefixAsync("featured_listings_");
    }
}
