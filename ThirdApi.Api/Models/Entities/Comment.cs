namespace Blog.Api.Models.Entities;

public class Comment : BaseEntity
    {
    public Guid PostId { get; set; }
    public string Author { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    }