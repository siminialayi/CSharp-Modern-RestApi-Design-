namespace Blog.Api.Models.DataTransferObjets.RequestDtos;

/// <summary>
/// Data Transfer Object for creating or updating a Comment.
/// </summary>
public class CommentRequestDto
    {
    /// <summary>
    /// Gets or sets the content of the comment.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier of the Post the comment belongs to.
    /// </summary>
    public Guid PostId { get; set; }
    }