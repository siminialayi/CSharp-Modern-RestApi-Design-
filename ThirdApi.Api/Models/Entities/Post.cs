namespace Blog.Api.Models.Entities;

public class Post : BaseEntity
    {
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    }