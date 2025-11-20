using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstatePlatform.Application.Common.Models;
using RealEstatePlatform.Application.Interfaces.Repositories;
using RealEstatePlatform.Domain.Entities;
using RealEstatePlatform.Domain.Enums;

namespace RealEstatePlatform.API.Controllers.Admin;

[ApiController]
[Route("api/admin/[controller]")]
[Authorize(Roles = "Admin,Moderator")]
public class DashboardController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        ILogger<DashboardController> logger)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet("stats")]
    [ProducesResponseType(typeof(ApiResponse<DashboardStatsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardStats(CancellationToken cancellationToken = default)
    {
        try
        {
            var totalUsers = _userManager.Users.Count(u => !u.IsDeleted);
            var activeUsers = _userManager.Users.Count(u => !u.IsDeleted && u.EmailConfirmed);
            
            var allListings = await _unitOfWork.PropertyListings.GetAllAsync(cancellationToken);
            var totalListings = allListings.Count(l => !l.IsDeleted);
            var activeListings = allListings.Count(l => !l.IsDeleted && l.Status == ListingStatus.Active);
            var pendingListings = allListings.Count(l => !l.IsDeleted && l.Status == ListingStatus.Pending);

            var allReviews = await _unitOfWork.Reviews.GetAllAsync(cancellationToken);
            var totalReviews = allReviews.Count(r => !r.IsDeleted);

            var allContacts = await _unitOfWork.ContactRequests.GetAllAsync(cancellationToken);
            var totalContacts = allContacts.Count(c => !c.IsDeleted);
            var unrespondedContacts = allContacts.Count(c => !c.IsDeleted && c.ResponseDate == null);

            var stats = new DashboardStatsDto
            {
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                TotalListings = totalListings,
                ActiveListings = activeListings,
                PendingListings = pendingListings,
                TotalReviews = totalReviews,
                TotalContacts = totalContacts,
                UnrespondedContacts = unrespondedContacts
            };

            return Ok(ApiResponse<DashboardStatsDto>.SuccessResponse(stats));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard stats");
            return StatusCode(500, ApiResponse<DashboardStatsDto>.ErrorResponse("An error occurred"));
        }
    }

    [HttpGet("monthly-data")]
    [ProducesResponseType(typeof(ApiResponse<List<MonthlyDataDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMonthlyData([FromQuery] int months = 12, CancellationToken cancellationToken = default)
    {
        try
        {
            var startDate = DateTime.UtcNow.AddMonths(-months);
            var monthlyData = new List<MonthlyDataDto>();

            for (int i = months - 1; i >= 0; i--)
            {
                var monthStart = DateTime.UtcNow.AddMonths(-i).Date.AddDays(1 - DateTime.UtcNow.Day);
                var monthEnd = monthStart.AddMonths(1);

                var usersCount = _userManager.Users.Count(u => 
                    u.CreatedAt >= monthStart && u.CreatedAt < monthEnd && !u.IsDeleted);

                var allListings = await _unitOfWork.PropertyListings.GetAllAsync(cancellationToken);
                var listingsCount = allListings.Count(l => 
                    l.CreatedAt >= monthStart && l.CreatedAt < monthEnd && !l.IsDeleted);

                monthlyData.Add(new MonthlyDataDto
                {
                    Month = monthStart.ToString("MMM yyyy"),
                    Users = usersCount,
                    Listings = listingsCount
                });
            }

            return Ok(ApiResponse<List<MonthlyDataDto>>.SuccessResponse(monthlyData));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving monthly data");
            return StatusCode(500, ApiResponse<List<MonthlyDataDto>>.ErrorResponse("An error occurred"));
        }
    }

    [HttpGet("popular-locations")]
    [ProducesResponseType(typeof(ApiResponse<List<PopularLocationDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPopularLocations([FromQuery] int top = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            var allListings = await _unitOfWork.PropertyListings.GetAllAsync(cancellationToken);
            var activeListings = allListings.Where(l => !l.IsDeleted && l.Status == ListingStatus.Active).ToList();

            var locationGroups = activeListings
                .GroupBy(l => new { l.Property?.Ward?.District?.Province?.Name })
                .Select(g => new PopularLocationDto
                {
                    LocationName = g.Key.Name ?? "Unknown",
                    ListingCount = g.Count(),
                    AveragePrice = g.Any() ? (decimal)g.Average(l => l.Price) : 0
                })
                .OrderByDescending(l => l.ListingCount)
                .Take(top)
                .ToList();

            return Ok(ApiResponse<List<PopularLocationDto>>.SuccessResponse(locationGroups));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving popular locations");
            return StatusCode(500, ApiResponse<List<PopularLocationDto>>.ErrorResponse("An error occurred"));
        }
    }

    [HttpGet("top-listings")]
    [ProducesResponseType(typeof(ApiResponse<List<TopListingDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTopListings([FromQuery] int top = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            var allListings = await _unitOfWork.PropertyListings.GetAllAsync(cancellationToken);
            var topListings = allListings
                .Where(l => !l.IsDeleted && l.Status == ListingStatus.Active)
                .OrderByDescending(l => l.ViewCount)
                .Take(top)
                .Select(l => new TopListingDto
                {
                    Id = l.Id,
                    Title = l.Property?.Title ?? "N/A",
                    Price = l.Price,
                    ViewCount = l.ViewCount,
                    CreatedAt = l.CreatedAt
                })
                .ToList();

            return Ok(ApiResponse<List<TopListingDto>>.SuccessResponse(topListings));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving top listings");
            return StatusCode(500, ApiResponse<List<TopListingDto>>.ErrorResponse("An error occurred"));
        }
    }
}

// DTOs for dashboard
public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalListings { get; set; }
    public int ActiveListings { get; set; }
    public int PendingListings { get; set; }
    public int TotalReviews { get; set; }
    public int TotalContacts { get; set; }
    public int UnrespondedContacts { get; set; }
}

public class MonthlyDataDto
{
    public string Month { get; set; } = string.Empty;
    public int Users { get; set; }
    public int Listings { get; set; }
}

public class PopularLocationDto
{
    public string LocationName { get; set; } = string.Empty;
    public int ListingCount { get; set; }
    public decimal AveragePrice { get; set; }
}

public class TopListingDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
