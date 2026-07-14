using System.Net;
using System.Text.Json;
using EmployeeManagement.Api.Dtos;
using EmployeeManagement.Exceptions;

namespace EmployeeManagement.Api.Middleware;

/// <summary>Maps domain exceptions to appropriate HTTP status codes and a JSON error body.</summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception exception)
        {
            HttpStatusCode statusCode = exception switch
            {
                EmployeeNotFoundException => HttpStatusCode.NotFound,
                DuplicateEmployeeException => HttpStatusCode.Conflict,
                ArgumentException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };

            if (statusCode == HttpStatusCode.InternalServerError)
            {
                _logger.LogError(exception, "Unhandled exception while processing {Path}", context.Request.Path);
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            string message = statusCode == HttpStatusCode.InternalServerError
                ? "An unexpected error occurred."
                : exception.Message;

            await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponse(message)));
        }
    }
}
