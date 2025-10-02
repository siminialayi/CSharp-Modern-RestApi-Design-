using Blog.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Persistence;

/// <summary>
/// Entity Framework Core database context for the Blog API domain model.
/// </summary>
/// <remarks>
/// RESPONSIBILITY:
/// - Materializes and tracks <see cref="Post"/> and <see cref="Comment"/> aggregates.
/// - Acts as a unit-of-work boundary for transactional consistency in a single HTTP request scope.
/// CONFIGURATION STRATEGY:
/// - Uses convention-based model building; explicit Fluent API / Data Annotations can be layered as complexity grows.
/// - DbSet properties expose collection-like access patterns facilitating LINQ querying.
/// LIFETIME:
/// - Registered with scoped lifetime (per-request) to ensure deterministic disposal and change tracking isolation.
/// EXTENSION GUIDELINES:
/// - Override OnModelCreating for custom constraints, indices, relationships.
/// - Introduce value converters for domain-specific types (e.g., strongly typed IDs) when needed.
/// </remarks>
public class BlogApiContext(DbContextOptions<BlogApiContext> options) : DbContext(options)
    {
    /// <summary>
    /// Collection of persisted <see cref="Post"/> entities.
    /// </summary>
    public DbSet<Post> Posts => Set<Post>();

    /// <summary>
    /// Collection of persisted <see cref="Comment"/> entities.
    /// </summary>
    public DbSet<Comment> Comments => Set<Comment>();
    }