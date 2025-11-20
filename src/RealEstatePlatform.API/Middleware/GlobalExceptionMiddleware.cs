using Microsoft.AspNetCore.Diagnostics;
using RealEstatePlatform.Application.Common.Models;
using System.Net;
using System.Text.Json;

namespace RealEstatePlatform.API.Middleware;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            KeyNotFoundException => new
            {
                statusCode = (int)HttpStatusCode.NotFound,
                message = "Resource not found",
                details = exception.Message
            },
            UnauthorizedAccessException => new
            {
                statusCode = (int)HttpStatusCode.Forbidden,
                message = "Access forbidden",
                details = exception.Message
            },
            ArgumentException or InvalidOperationException => new
            {
                statusCode = (int)HttpStatusCode.BadRequest,
                message = "Bad request",
                details = exception.Message
            },
            _ => new
            {
                statusCode = (int)HttpStatusCode.InternalServerError,
                message = "An internal server error occurred",
                details = exception.Message
            }
        };

        context.Response.StatusCode = response.statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

/// <summary>
/// Extension methods for GlobalExceptionMiddleware
/// </summary>
public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
