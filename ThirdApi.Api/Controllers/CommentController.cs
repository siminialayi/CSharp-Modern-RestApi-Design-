using Blog.Api.Models;
using Blog.Api.Models.DataTransferObjets.RequestDtos;
using Blog.Api.Services.CommentServices;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.Api.Controllers;

/// <summary>
/// Controller for managing comments, providing a fully asynchronous RESTful API interface.
/// </summary>
/// <remarks>
/// Utilizes the async/await pattern to ensure server scalability and efficient thread utilization 
/// when performing I/O operations.
/// </remarks>
/// <param name="commentService">The injected asynchronous comment business logic service.</param>
[Route("api/[controller]")]
[ApiController]
public class CommentController(ICommentService commentService) : ControllerBase
    {
    /// <summary>
    /// Retrieves a list of all comments asynchronously, returning DTOs.
    /// </summary>
    /// <returns>An asynchronous action result containing a list of comments (200 OK).</returns>
    [HttpGet]
    public async Task<IActionResult> Get()
        {
        // T
        var comments = await commentService.GetCommentsAsync();
        return Ok(comments);
        }

    /// <summary>
    /// Retrieves a specific comment by its unique identifier asynchronously, returning a DTO.
    /// </summary>
    /// <param name="id">The unique identifier of the comment.</param>
    /// <returns>An asynchronous action result containing the comment (200 OK) or 404 Not Found.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
        {
        // The service now returns a CommentResponseDto
        var comment = await commentService.GetByIdAsync(id);
        if (comment == null)
            {
            return NotFound($"Comment with id: {id} not found");
            }

        return Ok(comment);
        }

    // [REMAINDER OF METHODS (Post, Put, Delete) REMAIN UNCHANGED]

    /// <summary>
    /// Creates a new comment asynchronously.
    /// </summary>
    /// <param name="request">The data transfer object containing the comment details.</param>
    /// <returns>An asynchronous action result indicating successful creation (201 Created).</returns>
    [HttpPost]
    public async Task<IActionResult> Post(CommentRequestDto request)
        {
        // In a real JWT + Role-based system, Author is extracted from the authenticated user's claims.
        // Using a placeholder for demonstration.
        var author = User.FindFirstValue(ClaimTypes.Name) ?? "Anonymous/System User";

        await commentService.AddAsync(request, author);

        // RESTful best practice for POST: 201 Created.
        return CreatedAtAction(nameof(Get), new { id = Guid.Empty /* Placeholder for the actual ID */ }, "Comment added successfully");
        }

    /// <summary>
    /// Updates an existing comment asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the comment to update.</param>
    /// <param name="request">The data transfer object containing the updated comment details.</param>
    /// <returns>An asynchronous action result indicating success (200 OK) or 404 Not Found.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, CommentRequestDto request)
        {
        var updated = await commentService.UpdateAsync(id, request);
        if (!updated)
            {
            return NotFound($"Comment with id: {id} not found");
            }

        return Ok("Comment updated successfully");
        }

    /// <summary>
    /// Deletes a comment by its unique identifier asynchronously.
    /// </summary>
    /// <param name="id">The unique identifier of the comment to delete.</param>
    /// <returns>An asynchronous action result indicating success (204 No Content).</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
        {
        var deleted = await commentService.DeleteAsync(id);
        if (!deleted)
            {
            return NotFound($"Comment with id: {id} not found");
            }

        // Standard RESTful response for successful DELETE.
        return NoContent();
        }
    }