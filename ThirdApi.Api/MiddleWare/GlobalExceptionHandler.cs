using System.Net;
using Blog.Api.Models.ErrorResponses;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;

namespace Blog.Api.Middleware;

/// <summary>
/// Global exception handler using the IExceptionHandler service.
/// </summary>
public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
    // The main method that processes an unhandled exception
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
        {
        // 1. Log the exception for the ops team
        logger.LogError(
            exception, "Unhandled Exception caught globally. Path: {path}", httpContext.Request.GetDisplayUrl());

        // 2. Determine response details
        var statusCode = HttpStatusCode.InternalServerError;
        var title = "An unexpected error occurred.";
        var detail = "The server encountered an error. Please try again or contact support.";

        // Example of handling a specific business/domain exception (e.g., NotFound)
        if (exception is ArgumentNullException || exception is FileNotFoundException)
            {
            statusCode = HttpStatusCode.NotFound;
            title = "Resource Not Found";
            detail = exception.Message;
            }
        // NOTE: In a real app, you would handle custom exceptions (e.g., DomainException, ValidationException) here.

        // 3. Prepare the response object
        var response = new ErrorResponse
            {
            Status = statusCode,
            Title = title,
            Detail = detail,
            };

        // 4. Write the standardized JSON response
        httpContext.Response.StatusCode = (int)statusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        // Return true to indicate the exception has been handled and the pipeline should stop
        return true;
        }
    }