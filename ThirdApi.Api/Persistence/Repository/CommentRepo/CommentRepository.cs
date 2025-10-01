using Blog.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Persistence.Repository.CommentRepo;

/// <summary>
/// Provides asynchronous data access methods for Comment entities using Entity Framework Core.
/// </summary>
/// <remarks>
/// All I/O operations are asynchronous to prevent thread pool starvation and maximize application throughput.
/// </remarks>
/// <param name="context">The application's database context (Dependency Inversion Principle).</param>
public class CommentRepository(BlogApiContext context) : ICommentRepository
    {
    /// <summary>
    /// Retrieves all comments from the database asynchronously.
    /// </summary>
    public Task<List<Comment>> GetCommentsAsync() => context.Comments.ToListAsync();

    /// <summary>
    /// Finds a comment by its ID asynchronously.
    /// </summary>
    /// <param name="id">The comment ID.</param>
    public Task<Comment?> GetByIdAsync(Guid id) => context.Comments.FindAsync(id).AsTask();

    /// <summary>
    /// Adds a new comment to the database and saves changes asynchronously.
    /// </summary>
    /// <param name="comment">The comment to add.</param>
    public async Task AddAsync(Comment comment)
        {
        await context.Comments.AddAsync(comment);

        await context.SaveChangesAsync();
        }

    /// <summary>
    /// Updates an existing comment in the database and saves changes asynchronously.
    /// </summary>
    /// <param name="comment">The comment with updated values.</param>
    public async Task UpdateAsync(Comment comment)
        {
        context.Comments.Update(comment);
        await context.SaveChangesAsync();
        }

    /// <summary>
    /// Deletes a comment from the database and saves changes asynchronously.
    /// </summary>
    /// <param name="comment">The comment to delete.</param>
    public async Task DeleteAsync(Comment comment)
        {
        context.Comments.Remove(comment);
        await context.SaveChangesAsync();
        }
    }