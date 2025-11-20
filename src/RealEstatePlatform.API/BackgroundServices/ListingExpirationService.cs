using Microsoft.EntityFrameworkCore;
using RealEstatePlatform.Domain.Enums;
using RealEstatePlatform.Infrastructure.Data;

namespace RealEstatePlatform.API.BackgroundServices;

/// <summary>
/// Background service to check and expire old listings
/// </summary>
public class ListingExpirationService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ListingExpirationService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

    public ListingExpirationService(
        IServiceProvider serviceProvider,
        ILogger<ListingExpirationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Listing Expiration Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckExpiredListings(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Listing Expiration Service");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("Listing Expiration Service stopped");
    }

    private async Task CheckExpiredListings(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var now = DateTime.UtcNow;

        var expiredListings = await context.PropertyListings
            .Where(pl => 
                pl.Status == ListingStatus.Active &&
                pl.EndDate.HasValue &&
                pl.EndDate.Value < now)
            .ToListAsync(cancellationToken);

        if (expiredListings.Any())
        {
            _logger.LogInformation("Found {Count} expired listings", expiredListings.Count);

            foreach (var listing in expiredListings)
            {
                listing.Status = ListingStatus.Expired;
                listing.UpdatedAt = now;
            }

            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Marked {Count} listings as expired", expiredListings.Count);
        }
    }
}
