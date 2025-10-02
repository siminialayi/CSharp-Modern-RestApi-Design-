// File: Blog.Api/Models/CommentResponseDto.cs

namespace Blog.Api.Models.DataTransferObjets.ResponseDtos;

/// <summary>
/// Data Transfer Object for retrieving Comment details.
/// Hides the underlying entity structure and controls API output.
/// </summary>
public class CommentResponseDto
    {
    // XAML Comment: Unique identifier for the Comment.
    public Guid Id { get; set; }

    // XAML Comment: The ID of the Post this Comment belongs to.
    public Guid PostId { get; set; }

    // XAML Comment: The author's name of the Comment.
    public string Author { get; set; }

    // XAML Comment: The content of the Comment.
    public string Content { get; set; }

    // XAML Comment: Timestamp of when the Comment was created.
    public DateTime CreatedAt { get; set; }
    }