// File: Blog.Api/Models/DataTransferObjets/ResponseDtos/CommentResponseDto.cs

namespace Blog.Api.Models.DataTransferObjets.ResponseDtos;

/// <summary>
/// Data Transfer Object for exposing <see cref="Entities.Comment"/> data to API consumers.
/// </summary>
/// <remarks>
/// RATIONALE:
/// - Shields internal entity implementation, allowing evolution without breaking external contracts.
/// - Omits mutable server-managed properties (e.g., UpdatedAt) to reduce payload noise and avoid confusion.
/// - Serves as a projection target for mapping configuration (Mapster) enabling selective field inclusion.
/// EXTENSIBILITY:
/// - Add fields (e.g., UpdatedAt, IsEdited flag) when client use cases demand them rather than preemptively.
/// </remarks>
public class CommentResponseDto
    {
    /// <summary>
    /// Unique identifier of the comment.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Identifier of the parent <see cref="Entities.Post"/>.
    /// </summary>
    public Guid PostId { get; set; }

    /// <summary>
    /// Display name / username of the comment author.
    /// </summary>
    public string Author { get; set; }

    /// <summary>
    /// Text content of the comment.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// UTC timestamp indicating when the comment was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    }