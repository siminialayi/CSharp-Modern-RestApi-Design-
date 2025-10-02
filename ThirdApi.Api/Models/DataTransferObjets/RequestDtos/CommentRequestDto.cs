namespace Blog.Api.Models.DataTransferObjets.RequestDtos;

/// <summary>
/// Data Transfer Object for creating or updating a <see cref="Entities.Comment"/>.
/// </summary>
/// <remarks>
/// DESIGN INTENT:
/// - Separates external contract from internal entity to shield persistence concerns and prevent over-posting.
/// - Minimal surface (only mutable fields) to reduce accidental exposure of audit/identity information.
/// VALIDATION STRATEGY:
/// - Syntactic and semantic rules enforced by <c>CommentRequestDtoValidator</c> (FluentValidation) to keep DTO passive.
/// EXTENSIBILITY:
/// - Add fields (e.g., ParentCommentId for threading) carefully—ensure mapping configuration is updated accordingly.
/// </remarks>
public class CommentRequestDto
    {
    /// <summary>
    /// Body content of the comment supplied by the client.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Identifier of the <see cref="Entities.Post"/> the comment belongs to.
    /// </summary>
    public Guid PostId { get; set; }
    }