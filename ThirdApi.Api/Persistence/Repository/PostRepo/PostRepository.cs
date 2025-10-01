using Blog.Api.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Persistence.Repository.PostRepo;

public class PostRepository(BlogApiContext context) : IPostRepository
    {
    public List<Post> GetPosts() => context.Posts.ToList();

    public Post? GetById(Guid id)
        {
        return context.Posts.Find(id);
        }

    public void Add(Post post)
        {
        context.Add(post);
        context.SaveChanges();
        }

    public void Update(Post post)
        {
        context.Update(post);
        context.SaveChanges();
        }

    public void Delete(Post post)
        {
        context.Remove(post);
        context.SaveChanges();
        }
    }