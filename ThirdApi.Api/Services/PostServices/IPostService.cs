using Blog.Api.Models.DataTransferObjets.RequestDtos;
using Blog.Api.Models.Entities;

namespace Blog.Api.Services.PostServices;

public interface IPostService
    {
    void Add(PostRequestDto request);
    bool Delete(Guid id);
    Post? GetById(Guid id);
    List<Post> GetPosts();
    bool Update(Guid id, PostRequestDto request);
    }