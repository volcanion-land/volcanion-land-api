using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.DTOs.Listing;
using RealEstatePlatform.Application.Interfaces.Repositories;
using RealEstatePlatform.Domain.Entities;
using RealEstatePlatform.Domain.Enums;
using RealEstatePlatform.Infrastructure.Data;

namespace RealEstatePlatform.Infrastructure.Repositories;

/// <summary>
/// Property listing repository implementation
/// </summary>
public class PropertyListingRepository : Repository<PropertyListing>, IPropertyListingRepository
{
    public PropertyListingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PaginatedList<PropertyListing>> SearchListingsAsync(
        SearchFilter filter, 
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(pl => pl.Property)
                .ThenInclude(p => p.Images)
            .Include(pl => pl.Property)
                .ThenInclude(p => p.Ward)
                    .ThenInclude(w => w.District)
                        .ThenInclude(d => d.Province)
            .Include(pl => pl.Property)
                .ThenInclude(p => p.PropertyAmenities)
                    .ThenInclude(pa => pa.Amenity)
            .Where(pl => pl.Status == ListingStatus.Active);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            query = query.Where(pl => 
                EF.Functions.ILike(pl.Property.Title, $"%{filter.Keyword}%") ||
                EF.Functions.ILike(pl.Property.Description, $"%{filter.Keyword}%") ||
                EF.Functions.ILike(pl.Property.Address, $"%{filter.Keyword}%"));
        }

        if (filter.ListingType.HasValue)
        {
            query = query.Where(pl => pl.ListingType == filter.ListingType.Value);
        }

        if (filter.PropertyType.HasValue)
        {
            query = query.Where(pl => pl.Property.PropertyType == filter.PropertyType.Value);
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(pl => pl.Price >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(pl => pl.Price <= filter.MaxPrice.Value);
        }

        if (filter.MinArea.HasValue)
        {
            query = query.Where(pl => pl.Property.Area >= filter.MinArea.Value);
        }

        if (filter.MaxArea.HasValue)
        {
            query = query.Where(pl => pl.Property.Area <= filter.MaxArea.Value);
        }

        if (filter.Bedrooms.HasValue)
        {
            query = query.Where(pl => pl.Property.Bedrooms >= filter.Bedrooms.Value);
        }

        if (filter.Bathrooms.HasValue)
        {
            query = query.Where(pl => pl.Property.Bathrooms >= filter.Bathrooms.Value);
        }

        if (filter.WardId.HasValue)
        {
            query = query.Where(pl => pl.Property.WardId == filter.WardId.Value);
        }
        else if (filter.DistrictId.HasValue)
        {
            query = query.Where(pl => pl.Property.Ward.DistrictId == filter.DistrictId.Value);
        }
        else if (filter.ProvinceId.HasValue)
        {
            query = query.Where(pl => pl.Property.Ward.District.ProvinceId == filter.ProvinceId.Value);
        }

        if (filter.AmenityIds != null && filter.AmenityIds.Any())
        {
            query = query.Where(pl => 
                pl.Property.PropertyAmenities.Any(pa => filter.AmenityIds.Contains(pa.AmenityId)));
        }

        if (filter.IsFeatured.HasValue)
        {
            query = query.Where(pl => pl.IsFeatured == filter.IsFeatured.Value);
        }

        // Spatial search
        if (filter.Latitude.HasValue && filter.Longitude.HasValue && filter.RadiusKm.HasValue)
        {
            var point = new Point(filter.Longitude.Value, filter.Latitude.Value) { SRID = 4326 };
            var radiusMeters = filter.RadiusKm.Value * 1000;

            query = query.Where(pl => 
                pl.Property.Location != null && 
                pl.Property.Location.Distance(point) <= radiusMeters);
        }

        // Sorting
        query = filter.SortBy?.ToLower() switch
        {
            "price" => filter.SortDescending 
                ? query.OrderByDescending(pl => pl.Price) 
                : query.OrderBy(pl => pl.Price),
            "area" => filter.SortDescending 
                ? query.OrderByDescending(pl => pl.Property.Area) 
                : query.OrderBy(pl => pl.Property.Area),
            "createdat" => filter.SortDescending 
                ? query.OrderByDescending(pl => pl.CreatedAt) 
                : query.OrderBy(pl => pl.CreatedAt),
            _ => query.OrderByDescending(pl => pl.IsFeatured).ThenByDescending(pl => pl.CreatedAt)
        };

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination
        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<PropertyListing>(items, totalCount, filter.PageNumber, filter.PageSize);
    }

    public async Task<PropertyListing?> GetDetailAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(pl => pl.Property)
                .ThenInclude(p => p.Images.OrderBy(i => i.DisplayOrder))
            .Include(pl => pl.Property)
                .ThenInclude(p => p.Ward)
                    .ThenInclude(w => w.District)
                        .ThenInclude(d => d.Province)
            .Include(pl => pl.Property)
                .ThenInclude(p => p.PropertyAmenities)
                    .ThenInclude(pa => pa.Amenity)
            .Include(pl => pl.Property)
                .ThenInclude(p => p.Owner)
            .Include(pl => pl.User)
            .FirstOrDefaultAsync(pl => pl.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<PropertyListing>> GetFeaturedListingsAsync(
        int count = 10, 
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(pl => pl.Property)
                .ThenInclude(p => p.Images)
            .Include(pl => pl.Property)
                .ThenInclude(p => p.Ward)
                    .ThenInclude(w => w.District)
                        .ThenInclude(d => d.Province)
            .Where(pl => pl.Status == ListingStatus.Active && pl.IsFeatured)
            .OrderByDescending(pl => pl.CreatedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PropertyListing>> GetSimilarListingsAsync(
        Guid listingId, 
        int count = 5, 
        CancellationToken cancellationToken = default)
    {
        var listing = await GetByIdAsync(listingId, cancellationToken);
        if (listing == null) return Enumerable.Empty<PropertyListing>();

        return await _dbSet
            .Include(pl => pl.Property)
                .ThenInclude(p => p.Images)
            .Include(pl => pl.Property)
                .ThenInclude(p => p.Ward)
            .Where(pl => 
                pl.Id != listingId &&
                pl.Status == ListingStatus.Active &&
                pl.ListingType == listing.ListingType &&
                pl.Property.PropertyType == listing.Property.PropertyType)
            .OrderBy(pl => Math.Abs(pl.Price - listing.Price))
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<PropertyListing>> GetNearbyListingsAsync(
        double latitude, 
        double longitude, 
        double radiusKm, 
        int count = 10, 
        CancellationToken cancellationToken = default)
    {
        var point = new Point(longitude, latitude) { SRID = 4326 };
        var radiusMeters = radiusKm * 1000;

        return await _dbSet
            .Include(pl => pl.Property)
                .ThenInclude(p => p.Images)
            .Include(pl => pl.Property)
                .ThenInclude(p => p.Ward)
            .Where(pl => 
                pl.Status == ListingStatus.Active &&
                pl.Property.Location != null &&
                pl.Property.Location.Distance(point) <= radiusMeters)
            .OrderBy(pl => pl.Property.Location!.Distance(point))
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task IncrementViewCountAsync(Guid listingId, CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE \"PropertyListings\" SET \"ViewCount\" = \"ViewCount\" + 1 WHERE \"Id\" = {listingId}",
            cancellationToken);
    }
}
