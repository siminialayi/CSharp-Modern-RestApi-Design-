namespace Blog.Api.Models.Entities;

/// <summary>
/// Represents a user-authored remark associated with a blog <see cref="Post"/>.
/// </summary>
/// <remarks>
/// DOMAIN & DESIGN INTENT:
/// - Inherits audit + identity metadata from <see cref="BaseEntity"/>.
/// - Author is captured as a string for simplicity; in a richer model this would reference a User entity (FK/UserId).
/// - Content kept as string; consider length constraints / full-text indexing at persistence layer if search scenarios emerge.
/// - <see cref="PostId"/> is a required foreign key establishing many-to-one relation (Post -> Comments).
/// EVOLUTION GUIDELINES:
/// - Add moderation fields (IsApproved, FlagCount) for community governance.
/// - Add navigation property Post when lazy/eager loading patterns are introduced.
/// - Add UpdatedBy to complement UpdatedAt for audit trails in authenticated systems.
/// </remarks>
public class Comment : BaseEntity
    {
    /// <summary>
    /// Foreign key referencing the parent <see cref="Post"/>.
    /// </summary>
    public Guid PostId { get; set; }

    /// <summary>
    /// Display name or username of the principal who authored the comment.
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// Body text content of the comment.
    /// </summary>
    public string Content { get; set; } = string.Empty;
    }