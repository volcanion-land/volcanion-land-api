using Microsoft.Extensions.Diagnostics.HealthChecks;
using RealEstatePlatform.Infrastructure.Data;

namespace RealEstatePlatform.API.HealthChecks;

/// <summary>
/// Custom health check for database
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly ApplicationDbContext _context;

    public DatabaseHealthCheck(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.CanConnectAsync(cancellationToken);
            return HealthCheckResult.Healthy("Database connection is healthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database connection is unhealthy", ex);
        }
    }
}
