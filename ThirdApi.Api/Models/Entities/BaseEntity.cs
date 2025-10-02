namespace Blog.Api.Models.Entities;

/// <summary>
/// Provides common persistence metadata for aggregate and entity root types.
/// </summary>
/// <remarks>
/// DESIGN NOTES:
/// - <see cref="Id"/> is initialized at construction time using <see cref="Guid.NewGuid"/> ensuring deterministic identity before persistence.
/// - <see cref="CreatedAt"/> and <see cref="UpdatedAt"/> use UTC for time zone neutrality and audit reliability.
/// - <c>abstract</c> to prevent direct instantiation; only concrete domain entities should be materialized.
/// EXTENSION GUIDELINES:
/// - Add soft-delete flags (e.g., IsDeleted, DeletedAt) here if business rules require logical deletion.
/// - Add concurrency token (e.g., RowVersion byte[]) for optimistic concurrency checks if needed.
/// </remarks>
public abstract class BaseEntity
    {
    /// <summary>
    /// Globally unique identifier for the entity instance (value generated client-side).
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// UTC timestamp marking when the entity was first instantiated/persisted.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// UTC timestamp marking the latest mutation of the entity's state.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }