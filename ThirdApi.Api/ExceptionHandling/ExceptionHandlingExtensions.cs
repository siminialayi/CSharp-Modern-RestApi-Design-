using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.ExceptionHandling;

/// <summary>
/// Provides extension methods to register and use a global exception handling middleware pattern.
/// </summary>
/// <remarks>
/// DESIGN INTENT:
/// - Centralizes unhandled exception translation into <see cref="ProblemDetails"/> responses.
/// - Avoids scattering try/catch blocks across controllers/services; encourages fail-fast semantics.
/// - Conforms to RFC 7807 for machine-readable error payloads.
/// IMPLEMENTATION NOTES:
/// - Uses <c>UseExceptionHandler</c> to short-circuit pipeline on unhandled exceptions and generate a uniform response.
/// - In Development environment the full exception.ToString() is exposed to accelerate debugging; in other environments only the message is leaked to reduce information disclosure risk.
/// EXTENSION GUIDELINES:
/// - Introduce domain-specific exception types (e.g., DomainValidationException) and map them to 400/422 status codes here.
/// - Add correlation ID enrichment (e.g., from trace context) to the ProblemDetails for distributed traceability.
/// SECURITY CONSIDERATION:
/// - Ensure sensitive data (connection strings, PII) is never included in exception messages propagated to clients.
/// </remarks>
public static class ExceptionHandlingExtensions
{
    /// <summary>
    /// Registers ProblemDetails and related services (if not already registered) for standardized error responses.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddExceptionHandlingServices(this IServiceCollection services)
    {
        // Ensure ProblemDetails is registered (idempotent call)
        services.AddProblemDetails();
        return services;
    }

    /// <summary>
    /// Adds a simple exception handling middleware that returns standardized ProblemDetails responses.
    /// </summary>
    /// <param name="app">The web application pipeline builder.</param>
    /// <param name="env">The hosting environment (used to show detailed errors only in Development).</param>
    /// <returns>The same application instance for chaining.</returns>
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseExceptionHandler(handlerApp =>
        {
            handlerApp.Run(async context =>
            {
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (exceptionHandlerFeature is null)
                    return;

                var exception = exceptionHandlerFeature.Error;

                var problemDetails = new ProblemDetails
                {
                    Title = "An unexpected error occurred.",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = env.IsDevelopment() ? exception.ToString() : exception.Message,
                    Instance = context.Request.Path
                };

                context.Response.StatusCode = problemDetails.Status.Value;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(problemDetails);
            });
        });

        return app;
    }
}
