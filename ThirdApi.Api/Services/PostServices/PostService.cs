using Blog.Api.Models;
using Blog.Api.Models.DataTransferObjets.RequestDtos;
using Blog.Api.Models.Entities;
using Blog.Api.Persistence.Repository.PostRepo;

namespace Blog.Api.Services.PostServices;

public class PostService(IPostRepository postRepository) : IPostService
    {
    public List<Post> GetPosts() => postRepository.GetPosts();

    public Post? GetById(Guid id) => postRepository.GetById(id);

    public void Add(PostRequestDto request)
        {
        var post = new Post
            {
            Title = request.Title,
            Content = request.Content
            };

        postRepository.Add(post);
        }

    public bool Update(Guid id, PostRequestDto request)
        {
        var post = postRepository.GetById(id);
        if (post == null)
            {
            return false;
            }

        post.Title = request.Title;
        post.Content = request.Content;
        post.UpdatedAt = DateTime.UtcNow;

        postRepository.Update(post);

        return true;
        }

    public bool Delete(Guid id)
        {
        var post = postRepository.GetById(id);
        if (post == null)
            {
            return false;
            }

        postRepository.Delete(post);
        return true;
        }
    }