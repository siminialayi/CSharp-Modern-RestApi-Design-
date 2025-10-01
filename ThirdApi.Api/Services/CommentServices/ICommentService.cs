using Blog.Api.Models;
using Blog.Api.Models.DataTransferObjets.RequestDtos;
using Blog.Api.Models.DataTransferObjets.ResponseDtos;
using Blog.Api.Models.Entities;

namespace Blog.Api.Services.CommentServices;

/// <summary>
/// Defines the asynchronous contract for Comment business logic operations.
/// </summary>
public interface ICommentService
    {
    /// <summary>
    /// Retrieves all comments asynchronously.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation, with a list of Comment entities.</returns>
    Task<List<CommentResponseDto>> GetCommentsAsync();

    /// <summary>
    /// Retrieves a specific comment by ID asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the comment.</param>
    /// <returns>A Task representing the asynchronous operation, with the Comment entity or null.</returns>
    Task<CommentResponseDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Adds a new comment based on the request DTO asynchronously.
    /// </summary>
    /// <param name="request">The comment creation request data.</param>
    /// <param name="author">The author of the comment (from authenticated user).</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task AddAsync(CommentRequestDto request, string author);

    /// <summary>
    /// Updates an existing comment asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the comment to update.</param>
    /// <param name="request">The comment update request data.</param>
    /// <returns>A Task representing the asynchronous operation. The Task result is true if updated, false if not found.</returns>
    Task<bool> UpdateAsync(Guid id, CommentRequestDto request);

    /// <summary>
    /// Deletes a comment by ID asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the comment to delete.</param>
    /// <returns>A Task representing the asynchronous operation. The Task result is true if deleted, false if not found.</returns>
    Task<bool> DeleteAsync(Guid id);
    }