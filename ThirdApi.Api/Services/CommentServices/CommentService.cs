using Blog.Api.Models.DataTransferObjets.RequestDtos;
using Blog.Api.Models.DataTransferObjets.ResponseDtos;
using Blog.Api.Models.Entities;
using Blog.Api.Persistence.Repository.CommentRepo;
using Mapster;
using MapsterMapper;

namespace Blog.Api.Services.CommentServices;

/// <summary>
/// Implements the ICommentService contract, housing the business logic for comment management.
/// </summary>
/// <remarks>
/// All methods use the async pattern, delegating asynchronous I/O to the repository, 
/// upholding the principle of Separation of Concerns (SRP).
/// </remarks>
/// <param name="commentRepository">The injected asynchronous data access repository.</param>
public class CommentService(ICommentRepository commentRepository, IMapper mapper) : ICommentService
    {
    /// <summary>
    /// Gets all comments asynchronously and maps them to CommentResponseDto.
    /// </summary>
    public async Task<List<CommentResponseDto>> GetCommentsAsync()
        {
        var comments = await commentRepository.GetCommentsAsync();
        return mapper.Map<List<CommentResponseDto>>(comments);
        }

    /// <summary>
    /// Gets a comment by its ID asynchronously and maps it to CommentResponseDto.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    public async Task<CommentResponseDto?> GetByIdAsync(Guid id)
        {
        var comment = await commentRepository.GetByIdAsync(id);
        if (comment == null)
            {
            return null;
            }
        return mapper.Map<CommentResponseDto>(comment);
        }

    /// <summary>
    /// Adds a new comment to a post asynchronously.
    /// </summary>
    /// <param name="request">The DTO containing the comment details.</param>
    /// <param name="author">The author's name or username.</param>
    public async Task AddAsync(CommentRequestDto request, string author)
        {
        var comment = mapper.Map<Comment>(request);
        // Service-level enrichment
        comment.Author = author;
        await commentRepository.AddAsync(comment);
        }

    /// <summary>
    /// Updates an existing comment by ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the comment to update.</param>
    /// <param name="request">The DTO with updated content.</param>
    /// <returns>True if the update was successful; otherwise, false.</returns>
    public async Task<bool> UpdateAsync(Guid id, CommentRequestDto request)
        {
        var comment = await commentRepository.GetByIdAsync(id);
        if (comment == null)
            {
            return false;
            }

        request.Adapt(comment); // Maps allowed fields
        comment.UpdatedAt = DateTime.UtcNow; // business rule

        await commentRepository.UpdateAsync(comment);
        return true;
        }

    /// <summary>
    /// Deletes a comment by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the comment to delete.</param>
    /// <returns>True if the comment was found and deleted; otherwise, false.</returns>
    public async Task<bool> DeleteAsync(Guid id)
        {
        var comment = await commentRepository.GetByIdAsync(id);
        if (comment == null)
            {
            return false;
            }

        await commentRepository.DeleteAsync(comment);
        return true;
        }
    }