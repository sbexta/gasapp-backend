using FluentValidation;
using GasApp.Domain.Exceptions;
using System.Text.Json;

namespace GasApp.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, errorCode, message) = exception switch
        {
            ValidationException ve => (
                StatusCodes.Status422UnprocessableEntity,
                "VALIDATION_ERROR",
                string.Join("; ", ve.Errors.Select(e => e.ErrorMessage))),

            NotFoundException nfe => (
                StatusCodes.Status404NotFound,
                "NOT_FOUND",
                nfe.Message),

            DomainException de => (
                StatusCodes.Status400BadRequest,
                "DOMAIN_ERROR",
                de.Message),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                "UNAUTHORIZED",
                "No autorizado."),

            _ => (
                StatusCodes.Status500InternalServerError,
                "INTERNAL_ERROR",
                "Ocurrió un error interno. Intente de nuevo más tarde.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Error no controlado");

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new { errorCode, message };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
