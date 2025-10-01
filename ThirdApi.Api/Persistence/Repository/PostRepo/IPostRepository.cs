using Blog.Api.Models.Entities;

namespace Blog.Api.Persistence.Repository.PostRepo;

public interface IPostRepository
    {
    void Add(Post post);
    void Delete(Post post);
    Post? GetById(Guid id);
    List<Post> GetPosts();
    void Update(Post post);
    }