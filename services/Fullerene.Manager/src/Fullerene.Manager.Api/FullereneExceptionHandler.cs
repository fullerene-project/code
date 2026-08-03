using Fullerene.Shared.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Fullerene.Manager.Api;

public sealed class FullereneExceptionHandler(
    ILogger<FullereneExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
        Exception exception, CancellationToken ct)
    {
        logger.LogError(exception, "Unhandled exception caught.");
        
        httpContext.Response.StatusCode = exception switch
        {
            InternalException => StatusCodes.Status500InternalServerError,
            InvariantViolationException => StatusCodes.Status409Conflict,
            NotFoundException => StatusCodes.Status404NotFound,
            ValidationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };

        var detail =
            httpContext.Response.StatusCode == StatusCodes.Status500InternalServerError 
                ? "Internal server error." 
                : exception.Message;
        
        httpContext.Response.ContentType = "application/problem+json";
        
        var problemDetails = new ProblemDetails
        {
            Status = httpContext.Response.StatusCode,
            Detail = detail
        };

        if (exception is ValidationException validationException &&
            validationException.Errors.Count > 0)
        {
            problemDetails.Extensions["errors"] = validationException.Errors;
        }

        await httpContext.Response.WriteAsJsonAsync(problemDetails, ct);
        
        return true;
    }
}