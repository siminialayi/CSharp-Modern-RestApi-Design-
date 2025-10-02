using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.ExceptionHandling;

/// <summary>
/// Centralized exception handling extensions for the API hosting layer.
/// </summary>
/// <remarks>
/// <para><b>Purpose:</b> Encapsulates registration and activation of a uniform, opinionated exception handling strategy.
/// By pushing this into extensions we: (1) keep <c>Program.cs</c> expressive and declarative, (2) guarantee consistent ProblemDetails formatting,
/// (3) prevent ad-hoc <c>try/catch</c> blocks that erode readability and correctness.</para>
/// <para><b>Standards & Protocols:</b> Responses conform to <see href="https://datatracker.ietf.org/doc/html/rfc7807">RFC 7807 (Problem Details for HTTP APIs)</see> enabling machine-readable error contracts and easier client implementation.</para>
/// <para><b>Design Principles Applied:</b>
/// SOLID (Single Responsibility: one place owns transformation of unhandled exceptions to HTTP); DRY (no repeated controller boilerplate);
/// Fail-Fast (exceptions bubble immediately, are converted once); Information Hiding (internal exception types not leaked unless Development);
/// Security by Design (avoid exposing stack traces outside Development).</para>
/// <para><b>Extensibility Strategy:</b> This module starts intentionally minimal. Future evolution points:
/// (1) Map domain / validation / authorization exceptions to differentiated HTTP status codes;
/// (2) Enrich ProblemDetails with correlation IDs (traceparent), user context, or support links;
/// (3) Plug structured logging (e.g. Serilog) with event IDs before writing the response;
/// (4) Introduce retry-hint metadata for transient fault classification.</para>
/// <para><b>Operational Considerations:</b> Since this handler executes late in the pipeline (after other middleware), ensure any middleware that could throw (authN, authZ, body parsing) runs before registration so they are captured. Place AFTER routing but BEFORE endpoints when using minimal APIs for maximum coverage.</para>
/// <para><b>Why Not Use DeveloperExceptionPage in Production?</b> It reveals sensitive stack information. We consciously emulate its diagnostic richness ONLY in Development using <see cref="IWebHostEnvironment.IsDevelopment"/>.</para>
/// <para><b>Error Shape Rationale:</b>
/// Title: Stable, generic message avoiding internal jargon.
/// Status: Mirrors HTTP status for symmetry.
/// Detail: Full <c>exception.ToString()</c> only during Development for root cause speed; message otherwise to minimize information disclosure.
/// Instance: Request path – aids quick log correlation when coupled with path-based metrics.</para>
/// <para><b>Testing Guidance:</b> Unit test by invoking a minimal host with a test exception endpoint and asserting ProblemDetails JSON schema. Integration tests should assert (a) structure, (b) status code, (c) absence of stack trace outside Development.</para>
/// <para><b>Performance:</b> Overhead is negligible—only activated on exceptional flows. JSON serialization of a tiny object is minimal relative to request cost.</para>
/// <para><b>Audit Trail:</b> If regulatory logging is needed, insert logging BEFORE writing the response to ensure failures in logging do not corrupt the response stream.</para>
/// </remarks>
public static class ExceptionHandlingExtensions
{
    /// <summary>
    /// Registers ProblemDetails and related services supporting standardized error payloads.
    /// </summary>
    /// <param name="services">Application service collection.</param>
    /// <returns>The original service collection for fluent chaining.</returns>
    /// <remarks>
    /// <para><b>Idempotency:</b> Calling <see cref="ProblemDetailsServiceCollectionExtensions.AddProblemDetails"/> repeatedly is safe; underlying DI guards duplicate registrations.</para>
    /// <para><b>Reason for Separate Method:</b> Keeps Program bootstrap more intention revealing (<c>builder.Services.AddExceptionHandlingServices()</c>) and allows future enrichment (e.g., custom ProblemDetails factories) without touching callers.</para>
    /// </remarks>
    public static IServiceCollection AddExceptionHandlingServices(this IServiceCollection services)
    {
        // Framework helper registers ProblemDetails infrastructure (formatters / options).
        services.AddProblemDetails();
        return services;
    }

    /// <summary>
    /// Wires a global exception interception boundary that converts unhandled exceptions into standardized <see cref="ProblemDetails"/> responses.
    /// </summary>
    /// <param name="app">Application pipeline builder.</param>
    /// <param name="env">Hosting environment (used to decide detail verbosity).</param>
    /// <returns>The same <see cref="IApplicationBuilder"/> for chaining.</returns>
    /// <remarks>
    /// <para><b>Pipeline Mechanics:</b> <c>UseExceptionHandler</c> registers a branch. When an exception escapes downstream middleware, the request is re-executed on this branch and normal pipeline short-circuits.</para>
    /// <para><b>Why Inline Lambda Instead of Separate Endpoint:</b> Keeps behavior colocated and reduces surface area while simple. If complexity grows (multiple exception type mappings), refactor into a dedicated class.</para>
    /// <para><b>Content Negotiation:</b> We directly emit <c>application/problem+json</c> rather than relying on MVC negotiation to reduce stack depth and ensure RFC 7807 media type compliance.</para>
    /// <para><b>Minimal Exposure Policy:</b> Outside Development we provide only <c>exception.Message</c> (still potentially sanitized by domain layers). Avoid stack traces or internal type names.</para>
    /// <para><b>Extension Point:</b> Replace or augment the creation of <see cref="ProblemDetails"/> with a custom derivative adding fields such as trace IDs (<c>Activity.Current?.Id</c>) or error codes.</para>
    /// </remarks>
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseExceptionHandler(handlerApp =>
        {
            handlerApp.Run(async context =>
            {
                // Capture the feature populated by the framework when an unhandled exception bubbles out.
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (exceptionHandlerFeature is null)
                    return; // Defensive: Should not happen but avoids NullReferenceException if feature unavailable.

                var exception = exceptionHandlerFeature.Error; // Original root exception (already unwrapped once by hosting layer).

                // Construct contract-compliant problem payload.
                var problemDetails = new ProblemDetails
                {
                    Title = "An unexpected error occurred.", // Stable, client-safe banner.
                    Status = StatusCodes.Status500InternalServerError, // 500 chosen because only unhandled exceptions reach here presently.
                    Detail = env.IsDevelopment() ? exception.ToString() : exception.Message, // Rich diagnostic only in Development.
                    Instance = context.Request.Path // Correlates error to request path; complements external logging.
                };

                // Prepare response envelope.
                context.Response.StatusCode = problemDetails.Status.Value;
                context.Response.ContentType = "application/problem+json"; // Explicit media type per RFC 7807.

                // Emit JSON using built-in serializer honoring ProblemDetails conventions.
                await context.Response.WriteAsJsonAsync(problemDetails);
            });
        });

        return app; // Enables fluent registration pattern.
    }
}
