using System.Net;
using System.Text.Json;
using TourApp.Shared;

namespace TourApp.Api.Middleware;

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

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, errorCode) = MapExceptionToStatusCode(exception);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.Fail(
            errorCode,
            exception.Message,
            exception.StackTrace
        );

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return context.Response.WriteAsJsonAsync(response, options);
    }

    private static (HttpStatusCode statusCode, string errorCode) MapExceptionToStatusCode(Exception exception)
    {
        return exception switch
        {
            ArgumentException => (HttpStatusCode.BadRequest, "BAD_REQUEST"),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "UNAUTHORIZED"),
            KeyNotFoundException => (HttpStatusCode.NotFound, "NOT_FOUND"),
            InvalidOperationException => (HttpStatusCode.Conflict, "CONFLICT"),
            _ => (HttpStatusCode.InternalServerError, "INTERNAL_ERROR")
        };
    }
}
