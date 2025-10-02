namespace Blog.Api.Models.Entities;

/// <summary>
/// Represents a publishable blog post aggregate root.
/// </summary>
/// <remarks>
/// AGGREGATE ROLE:
/// - Serves as the parent for related <see cref="Comment"/> entities (one-to-many relationship).
/// - Encapsulates content fields; business rules (e.g., versioning, publishing workflow) can be layered in service layer.
/// IMPLEMENTATION NOTES:
/// - Inherits identity/audit metadata from <see cref="BaseEntity"/>.
/// - Title & Content kept simple; consider adding Slug, Summary, Tags, or Category references as domain evolves.
/// - For large content payloads, evaluate moving Content to a separate table or external store (e.g., blob) if size impacts query performance.
/// </remarks>
public class Post : BaseEntity
    {
    /// <summary>
    /// Human-readable title of the post.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Rich or plain text body content of the post.
    /// </summary>
    public string Content { get; set; } = string.Empty;
    }