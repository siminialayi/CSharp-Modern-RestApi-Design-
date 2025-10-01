using Blog.Api.Models.Entities;

namespace Blog.Api.Persistence.Repository.CommentRepo;

/// <summary>
/// Defines the asynchronous contract for Comment data access operations.
/// </summary>
public interface ICommentRepository
    {
    /// <summary>
    /// Retrieves a list of all comments asynchronously.
    /// </summary>
    /// <returns>A Task that represents the asynchronous operation. The Task result contains a list of Comment entities.</returns>
    Task<List<Comment>> GetCommentsAsync();

    /// <summary>
    /// Retrieves a specific comment by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the comment.</param>
    /// <returns>A Task that represents the asynchronous operation. The Task result contains the Comment entity, or null if not found.</returns>
    Task<Comment?> GetByIdAsync(Guid id);

    /// <summary>
    /// Adds a new comment entity to the database asynchronously.
    /// </summary>
    /// <param name="comment">The comment entity to add.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    Task AddAsync(Comment comment);

    /// <summary>
    /// Updates an existing comment entity in the database asynchronously.
    /// </summary>
    /// <param name="comment">The comment entity with updated values.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    Task UpdateAsync(Comment comment);

    /// <summary>
    /// Deletes a comment entity from the database asynchronously.
    /// </summary>
    /// <param name="comment">The comment entity to delete.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    Task DeleteAsync(Comment comment);
    }