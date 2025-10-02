using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using System.Reflection;

namespace Blog.Api.Configurations.Logging;

/// <summary>
/// Provides extension methods to configure Serilog logging for the application host.
/// </summary>
/// <remarks>
/// DESIGN INTENT:
/// Centralizes all logging pipeline concerns so the <c>Program</c> file remains a pure composition root.
/// This method is invoked early during host building so that DI, configuration, and environment context
/// are available before the application starts serving requests.
///
/// STRATEGY OVERVIEW:
/// 1. Configuration Driven: <see cref="LoggerConfiguration.ReadFrom.Configuration"/> allows external tuning
///    (level switches, sinks) without recompilation following the 12-Factor principle of config separation.
/// 2. Enrichers: Provide structured context for correlation, diagnostics, and distributed tracing bootstrap:
///    - MachineName: Distinguishes nodes in horizontal scaling.
///    - ThreadId: Aids debugging under parallel workloads.
///    - EnvironmentName: Partitions logs across environments (Dev, Staging, Prod).
///    - FromLogContext: Enables per-request scoping (e.g., adding correlation IDs later).
///    - Static Application property: Guarantees every event contains system identity for multi-app aggregation.
/// 3. Level Overrides: Noisy framework namespaces (Microsoft.*, ASP.NET Core) are suppressed to Warning+
///    to surface application-relevant signals while maintaining signal-to-noise ratio.
/// 4. Sinks:
///    a. Console (human readable) with concise template optimized for local dev & container logs.
///    b. Rolling plain text file (14-day retention) for quick tailing / ops review.
///    c. Compact JSON (<see cref="CompactJsonFormatter"/>) for structured ingestion by Seq / ELK / Loki.
/// 5. Environment Sensitivity: Development lowers MinimumLevel to Debug for richer interactive diagnostics;
///    higher environments default to Information to reduce volume and storage costs.
/// 6. Performance Considerations: Flush interval kept short (2s) to balance durability VS I/O overhead.
/// 7. Extension Format: Returns the same builder enabling fluent composition and testability.
///
/// EXTENSION GUIDELINES:
/// - To add distributed tracing IDs: add .Enrich.WithCorrelationId() via Serilog.Enrichers.CorrelationId.
/// - To add OpenTelemetry export: chain .WriteTo.OpenTelemetry() (requires additional package).
/// - To push to cloud sinks (Seq, Application Insights, etc.) append appropriate WriteTo calls guarded by config.
///
/// THREAD SAFETY: Serilog is thread-safe for logging operations after configuration is completed.
/// </remarks>
public static class SerilogExtensions
    {
    /// <summary>
    /// Configures Serilog for the application with standardized settings.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <returns>The configured web application builder with Serilog.</returns>
    /// <remarks>
    /// Adds console + rolling file + structured CLEF sinks.
    /// Enrichers add machine/environment/thread metadata for operational analysis.
    /// Level policy intentionally de-noises framework chatter unless elevated by environment.
    /// </remarks>
    public static WebApplicationBuilder AddSerilogConfiguration(this WebApplicationBuilder builder)
        {
        builder.Host.UseSerilog((context, loggerConfiguration) =>
        {
            loggerConfiguration
                // External configuration (appsettings.json / environment-specific variants)
                .ReadFrom.Configuration(context.Configuration) // requires Serilog.Settings.Configuration
                // --- Enrichment Layer ---
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithEnvironmentName()
                .Enrich.WithProperty("Application", Assembly.GetEntryAssembly()?.GetName().Name ?? "Blog.Api")
                // --- Noise Filtering Policy ---
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                // --- Console Sink (developer centric) ---
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                // --- Rolling Plain Text File Sink ---
                .WriteTo.File(
                    path: "logs/blog-api-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 14,
                    fileSizeLimitBytes: 10_000_000,
                    rollOnFileSizeLimit: true,
                    shared: false,
                    flushToDiskInterval: TimeSpan.FromSeconds(2),
                    outputTemplate:
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                // --- Structured CLEF Sink (machine-readable) ---
                .WriteTo.File(new CompactJsonFormatter(),
                    path: "logs/blog-api-structured-.clef",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7);

            // Environment-sensitive base level selection
            if (context.HostingEnvironment.IsDevelopment())
                {
                // Verbose granularity to accelerate feedback loops
                loggerConfiguration.MinimumLevel.Debug();
                }
            else
                {
                // Production-friendly baseline (elevate to Warning if volume high)
                loggerConfiguration.MinimumLevel.Information();
                }
        });

        return builder;
        }
    }