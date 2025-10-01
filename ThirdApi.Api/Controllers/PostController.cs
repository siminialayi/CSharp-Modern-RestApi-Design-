using Blog.Api.Models.DataTransferObjets.RequestDtos;
using Blog.Api.Services.PostServices;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PostController(IPostService postService) : ControllerBase
    {
    [HttpGet]
    public IActionResult Get()
        {
        var posts = postService.GetPosts();
        return Ok(posts);
        }

    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
        {
        var post = postService.GetById(id);
        if (post == null)
            {
            return NotFound($"Post with id: {id} not found");
            }

        return Ok(post);
        }

    [HttpPost]
    public IActionResult Post(PostRequestDto request)
        {
        postService.Add(request);
        return Ok("Post added successfully");
        }

    [HttpPut("{id}")]
    public IActionResult Put(Guid id, PostRequestDto request)
        {
        var updated = postService.Update(id, request);
        if (!updated)
            {
            return BadRequest($"Post with id: {id} not found");
            }

        return Ok("Post updated successfully");
        }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
        {
        var deleted = postService.Delete(id);
        if (!deleted)
            {
            return BadRequest($"Post with id: {id} not found");
            }

        return Ok("Post deleted successfully");
        }
    }