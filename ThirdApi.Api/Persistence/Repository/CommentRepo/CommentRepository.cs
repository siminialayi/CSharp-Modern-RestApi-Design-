using Blog.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Persistence.Repository.CommentRepo;

/// <summary>
/// Provides asynchronous data access methods for <see cref="Comment"/> entities using Entity Framework Core.
/// </summary>
/// <remarks>
/// ARCHITECTURAL ROLE:
/// - Implements the Repository pattern to abstract persistence concerns from higher application layers (Services / Controllers).
/// - Asynchronous APIs prevent thread pool starvation under high I/O latency workloads.
/// DESIGN DECISIONS:
/// - Uses DbContext tracking for standard CRUD (no AsNoTracking here since updates are common after retrieval in service layer).
/// - SaveChangesAsync invoked per operation for simplicity; batching / unit-of-work could be introduced if required for transactional boundaries.
/// TESTING STRATEGY:
/// - Replace with in-memory or SQLite provider in integration tests to verify query semantics.
/// EXTENSION GUIDELINES:
/// - Introduce specification pattern for complex filtering if query logic begins to aggregate.
/// - Add cancellation token parameters for cooperative cancellation in long-running requests.
/// </remarks>
/// <param name="context">The application's database context (Dependency Inversion Principle).</param>
public class CommentRepository(BlogApiContext context) : ICommentRepository
    {
    /// <inheritdoc />
    public Task<List<Comment>> GetCommentsAsync() => context.Comments.ToListAsync();

    /// <inheritdoc />
    public Task<Comment?> GetByIdAsync(Guid id) => context.Comments.FindAsync(id).AsTask();

    /// <inheritdoc />
    public async Task AddAsync(Comment comment)
        {
        await context.Comments.AddAsync(comment);
        await context.SaveChangesAsync();
        }

    /// <inheritdoc />
    public async Task UpdateAsync(Comment comment)
        {
        context.Comments.Update(comment);
        await context.SaveChangesAsync();
        }

    /// <inheritdoc />
    public async Task DeleteAsync(Comment comment)
        {
        context.Comments.Remove(comment);
        await context.SaveChangesAsync();
        }
    }