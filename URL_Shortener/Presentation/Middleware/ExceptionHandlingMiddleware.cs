using System.Net;
using System.Text.Json;

namespace URL_Shortener.Api.Middleware;

public class ExceptionHandlingMiddleware
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        HttpStatusCode statusCode;
        object responseBody;

        if (exception is FluentValidation.ValidationException validationException)
        {
            statusCode = HttpStatusCode.BadRequest;
            responseBody = new { errors = validationException.Errors.Select(e => e.ErrorMessage) };
        }
        else if (exception is InvalidOperationException || exception is ArgumentException)
        {
            statusCode = HttpStatusCode.BadRequest;
            responseBody = new { errors = new[] { exception.Message } };
        }
        else
        {
            statusCode = HttpStatusCode.InternalServerError;
            responseBody = new { errors = new[] { "An unexpected error occurred." } };
        }

        context.Response.StatusCode = (int)statusCode;
        var result = JsonSerializer.Serialize(responseBody);
        return context.Response.WriteAsync(result, context.RequestAborted);
    }
}
