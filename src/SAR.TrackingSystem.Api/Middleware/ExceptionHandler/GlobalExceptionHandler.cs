using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SAR.TrackingSystem.Api.Middleware.ExceptionHandler;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An error occurred");

        var problemDetails = new ProblemDetails
        {
            Title = "An error occurred",
            Status = StatusCodes.Status500InternalServerError,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        // special handling for ValidationException
        if (exception is ValidationException validationException)
        {
            var errors = validationException
                .Errors
                .Select(e => new { e.PropertyName, e.ErrorMessage, e.Severity });

            problemDetails.Extensions.Add("Errors", errors);
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Title = "Validation error";
        }

        httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);

        return true;
    }
}