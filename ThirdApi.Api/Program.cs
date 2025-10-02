using Blog.Api.Configurations.Logging;
using Blog.Api.ExceptionHandling; // <-- Added
using Blog.Api.Models.Validators;
using Blog.Api.Persistence;
using Blog.Api.Persistence.Repository.CommentRepo;
using Blog.Api.Persistence.Repository.PostRepo;
using Blog.Api.Services.CommentServices;
using Blog.Api.Services.PostServices;
using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Serilog;

/// <summary>
/// Application bootstrap entry-point for the Blog API.
/// </summary>
/// <remarks>
/// DESIGN OVERVIEW (acts as an architecture reference for future projects):
/// 1. Hosting & Startup: Minimal hosting model (.NET 8) with top-level statements. The Program file intentionally contains only composition logic (no business logic).
/// 2. Logging Pipeline: A two-stage Serilog pipeline is used.
///    a. Bootstrap logger (<see cref="Log.Logger"/>) captures failures during host building (e.g., configuration issues).
///    b. Full configuration is applied once DI, configuration providers, and environment context are available via <see cref="SerilogExtensions.AddSerilogConfiguration"/>.
/// 3. Dependency Injection: All layers (Repositories, Services, Validators, Mapping) are registered explicitly to make dependencies discoverable and encourage constructor injection.
/// 4. Data Layer: EF Core DbContext (<see cref="BlogApiContext"/>) registered with a SQL Server provider. Lifetime is scoped per request (AddDbContext default) to ensure unit-of-work semantics.
/// 5. Validation: FluentValidation integrated using automatic MVC pipeline adapters (ModelState population). Validators are discovered by assembly scanning for cohesive registration.
/// 6. Mapping: Mapster DI integration (<c>AddMapster()</c>) avoids reliance on static global configuration, keeping mappings testable and replaceable.
/// 7. Exception Handling: Centralized via <see cref="ExceptionHandlingExtensions.UseCustomExceptionHandler"/> producing RFC 7807 ProblemDetails responses.
/// 8. API Surfacing: Controllers remain thin; they delegate to services which encapsulate business rules and orchestration of repositories.
/// 9. Cross-Cutting Concerns: Logging, validation, mapping, and exception translation are configured once here and reused uniformly.
/// 10. Extensibility Guidance: Additional cross-cutting additions (e.g., caching, authentication, rate limiting) should be composed here to preserve the single composition root.
/// 11. Environment Awareness: Swagger only exposed in Development to mitigate accidental exposure in production deployments.
/// 12. Security Placeholder: Authorization middleware is present; authentication/authorization policies would be registered here when implemented.
/// 13. Performance Note: Serilog request logging template tuned to minimize allocation and includes elapsed time threshold-based level adjustment.
/// </remarks>

// Bootstrap logger: captures very early startup issues
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [BOOT] {Message:lj}{NewLine}{Exception}")
    .CreateBootstrapLogger();

try
    {
    Log.Information("Bootstrapping Blog API host");
    var builder = WebApplication.CreateBuilder(args);

    // Full Serilog configuration (attaches to Host)
    builder.AddSerilogConfiguration();

    // INFRASTRUCTURE: Database configuration (single source of truth for connection string via Configuration system)
    builder.Services.AddDbContext<BlogApiContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("BlogApiContext")));

    // REPOSITORIES (data access abstractions):
    builder.Services.AddScoped<IPostRepository, PostRepository>();
    builder.Services.AddScoped<ICommentRepository, CommentRepository>();

    // SERVICES (domain/business orchestration layer):
    builder.Services.AddScoped<IPostService, PostService>();
    builder.Services.AddScoped<ICommentService, CommentService>();

    // FRAMEWORK + PLATFORM SERVICES:
    builder.Services.AddControllers(); // Adds MVC Core + JSON formatters + routing
    builder.Services.AddEndpointsApiExplorer(); // Enables minimal endpoint metadata enumeration (used by Swagger)
    builder.Services.AddSwaggerGen(); // OpenAPI document generator

    // VALIDATION (FluentValidation integration into ASP.NET Core model binding pipeline)
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddFluentValidationClientsideAdapters();
    builder.Services.AddValidatorsFromAssemblyContaining<CommentRequestDtoValidator>(); // Assembly scan anchor type

    // MAPPING (Mapster) - centralized configuration via IRegister implementations
    builder.Services.AddMapster();

    // STANDARDIZED PROBLEM DETAILS + EXCEPTION HANDLING
    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandlingServices();

    var app = builder.Build();

    // GLOBAL EXCEPTION HANDLING MIDDLEWARE (before other middleware that might throw)
    app.UseCustomExceptionHandler(builder.Environment);

    if (app.Environment.IsDevelopment())
        {
        // API discoverability & interactive exploration (only in Dev to limit surface area)
        app.UseSwagger();
        app.UseSwaggerUI();
        }

    app.UseHttpsRedirection(); // Enforces TLS. Remove only for local non-HTTPS test harnesses.

    // Adds structured request logging (status code, elapsed ms, etc.)
    app.UseSerilogRequestLogging(options =>
    {
        // TEMPLATE DESIGN: Includes latency, method, path, and outcome classification
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        // DYNAMIC LEVEL SELECTION: Promotes slow (>1s) requests to Warning, failures to Error, rest Information
        options.GetLevel = (httpContext, elapsedMs, ex) =>
            ex != null || httpContext.Response.StatusCode >= 500
                ? Serilog.Events.LogEventLevel.Error
                : elapsedMs > 1000
                    ? Serilog.Events.LogEventLevel.Warning
                    : Serilog.Events.LogEventLevel.Information;
    });

    app.UseAuthorization(); // Placeholder: real authN/Z would precede controllers (e.g., UseAuthentication())
    app.MapControllers(); // Endpoint routing finalization

    Log.Information("Blog API application started successfully");
    app.Run();
    }
catch (Exception ex)
    {
    // LAST-CHANCE FAILSAFE: Ensures fatal exceptions are captured with context before process exit.
    Log.Fatal(ex, "Host terminated unexpectedly");
    }
finally
    {
    // Guarantees flushing of any buffered log events (important for file/async sinks)
    Log.CloseAndFlush();
    }