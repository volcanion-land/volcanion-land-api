using Prometheus;
using System.Diagnostics;

namespace RealEstatePlatform.API.Middleware;

/// <summary>
/// Middleware to collect Prometheus metrics
/// </summary>
public class PrometheusMetricsMiddleware
{
    private readonly RequestDelegate _next;
    
    private static readonly Counter RequestCounter = Metrics.CreateCounter(
        "http_requests_total",
        "Total HTTP requests",
        new CounterConfiguration
        {
            LabelNames = new[] { "method", "endpoint", "status_code" }
        });

    private static readonly Histogram RequestDuration = Metrics.CreateHistogram(
        "http_request_duration_seconds",
        "HTTP request duration in seconds",
        new HistogramConfiguration
        {
            LabelNames = new[] { "method", "endpoint" },
            Buckets = Histogram.ExponentialBuckets(0.001, 2, 10)
        });

    private static readonly Gauge ActiveRequests = Metrics.CreateGauge(
        "http_requests_active",
        "Number of active HTTP requests",
        new GaugeConfiguration
        {
            LabelNames = new[] { "method", "endpoint" }
        });

    public PrometheusMetricsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;
        var endpoint = context.Request.Path.Value ?? "/";
        
        // Simplify endpoint for metrics (remove IDs, GUIDs, etc.)
        endpoint = SimplifyEndpoint(endpoint);

        var stopwatch = Stopwatch.StartNew();
        
        using (ActiveRequests.WithLabels(method, endpoint).TrackInProgress())
        {
            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                
                var statusCode = context.Response.StatusCode.ToString();
                
                RequestCounter.WithLabels(method, endpoint, statusCode).Inc();
                RequestDuration.WithLabels(method, endpoint).Observe(stopwatch.Elapsed.TotalSeconds);
            }
        }
    }

    private static string SimplifyEndpoint(string endpoint)
    {
        // Replace GUIDs with placeholder
        var guidPattern = @"[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}";
        endpoint = System.Text.RegularExpressions.Regex.Replace(endpoint, guidPattern, "{id}");
        
        // Replace numeric IDs with placeholder
        var numericPattern = @"/\d+";
        endpoint = System.Text.RegularExpressions.Regex.Replace(endpoint, numericPattern, "/{id}");
        
        return endpoint;
    }
}

/// <summary>
/// Extension methods for PrometheusMetricsMiddleware
/// </summary>
public static class PrometheusMetricsMiddlewareExtensions
{
    public static IApplicationBuilder UsePrometheusMetrics(this IApplicationBuilder app)
    {
        return app.UseMiddleware<PrometheusMetricsMiddleware>();
    }
}
