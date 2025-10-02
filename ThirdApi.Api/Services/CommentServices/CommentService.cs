using Blog.Api.Models.DataTransferObjets.RequestDtos;
using Blog.Api.Models.DataTransferObjets.ResponseDtos;
using Blog.Api.Models.Entities;
using Blog.Api.Persistence.Repository.CommentRepo;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace Blog.Api.Services.CommentServices;

/// <summary>
/// Implements the <see cref="ICommentService"/> contract, housing business logic for comment lifecycle.
/// </summary>
/// <remarks>
/// ARCHITECTURAL ROLE:
/// - Orchestrates mapping, validation (implicit via model binding + validators), and persistence operations.
/// - Shields controllers from direct repository knowledge, enabling richer domain behaviors without API surface churn.
/// DESIGN PATTERNS:
/// - Repository pattern (via <see cref="ICommentRepository"/>) decouples persistence.
/// - Mapper (Mapster) isolates DTO <-> Entity transformations preventing over-posting vulnerabilities.
/// - Logging cross-cuts each operation with structured context for observability.
/// ERROR HANDLING STRATEGY:
/// - Exceptions are logged and rethrown to be surfaced by global exception middleware (ProblemDetails translation).
/// - Returns null / bool for not-found semantics instead of exceptions to preserve control flow clarity.
/// EVOLUTION GUIDELINES:
/// - Introduce domain events (e.g., CommentAdded) if external side effects (notifications, indexing) are needed.
/// - Add caching if read-heavy scenarios emerge; ensure invalidation on mutation paths.
/// </remarks>
public class CommentService : ICommentService
    {
    private readonly ICommentRepository _commentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CommentService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentService"/> class.
    /// </summary>
    /// <param name="commentRepository">The injected asynchronous data access repository.</param>
    /// <param name="mapper">The object mapper for DTO conversions.</param>
    /// <param name="logger">The logger for capturing service operations.</param>
    public CommentService(ICommentRepository commentRepository, IMapper mapper, ILogger<CommentService> logger)
        {
        _commentRepository = commentRepository;
        _mapper = mapper;
        _logger = logger;
        }

    /// <inheritdoc />
    public async Task<List<CommentResponseDto>> GetCommentsAsync()
        {
        _logger.LogInformation("Retrieving all comments");
        try
            {
            var comments = await _commentRepository.GetCommentsAsync();
            _logger.LogDebug("Retrieved {CommentCount} comments", comments.Count);
            return _mapper.Map<List<CommentResponseDto>>(comments);
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while retrieving comments");
            throw;
            }
        }

    /// <inheritdoc />
    public async Task<CommentResponseDto?> GetByIdAsync(Guid id)
        {
        _logger.LogInformation("Retrieving comment with ID: {CommentId}", id);
        try
            {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
                {
                _logger.LogWarning("Comment with ID: {CommentId} not found", id);
                return null;
                }
            _logger.LogDebug("Retrieved comment with ID: {CommentId}", id);
            return _mapper.Map<CommentResponseDto>(comment);
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while retrieving comment with ID: {CommentId}", id);
            throw;
            }
        }

    /// <inheritdoc />
    public async Task AddAsync(CommentRequestDto request, string author)
        {
        _logger.LogInformation("Adding new comment by author: {Author} to post: {PostId}", author, request.PostId);
        try
            {
            var comment = _mapper.Map<Comment>(request);
            // Service-level enrichment of server-managed fields
            comment.Author = author;
            await _commentRepository.AddAsync(comment);
            _logger.LogInformation("Successfully added comment with ID: {CommentId}", comment.Id);
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while adding comment by {Author} to post: {PostId}",
                author, request.PostId);
            throw;
            }
        }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(Guid id, CommentRequestDto request)
        {
        _logger.LogInformation("Updating comment with ID: {CommentId}", id);
        try
            {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
                {
                _logger.LogWarning("Comment with ID: {CommentId} not found for update", id);
                return false;
                }

            request.Adapt(comment); // Maps allowed mutable fields only
            comment.UpdatedAt = DateTime.UtcNow; // Business rule: update audit timestamp

            await _commentRepository.UpdateAsync(comment);
            _logger.LogInformation("Successfully updated comment with ID: {CommentId}", id);
            return true;
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while updating comment with ID: {CommentId}", id);
            throw;
            }
        }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id)
        {
        _logger.LogInformation("Deleting comment with ID: {CommentId}", id);
        try
            {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
                {
                _logger.LogWarning("Comment with ID: {CommentId} not found for deletion", id);
                return false;
                }

            await _commentRepository.DeleteAsync(comment);
            _logger.LogInformation("Successfully deleted comment with ID: {CommentId}", id);
            return true;
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while deleting comment with ID: {CommentId}", id);
            throw;
            }
        }
    }