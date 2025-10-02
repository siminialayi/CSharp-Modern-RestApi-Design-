using Blog.Api.Models;
using Blog.Api.Models.DataTransferObjets.RequestDtos;
using Blog.Api.Services.CommentServices;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.Api.Controllers;

/// <summary>
/// API surface for managing <see cref="Entities.Comment"/> resources.
/// </summary>
/// <remarks>
/// RESPONSIBILITY BOUNDARY:
/// - Translates HTTP concepts (routes, status codes) to service layer calls.
/// - Avoids embedding business logic; delegates to <see cref="ICommentService"/>.
/// - Returns standardized responses; error translation handled by global exception middleware.
/// DESIGN CHOICES:
/// - Asynchronous endpoints to maximize scalability for I/O-bound EF Core operations.
/// - CreatedAtAction used for POST to align with REST resource creation semantics.
/// EXTENSION GUIDELINES:
/// - Introduce authorization attributes (e.g., [Authorize]) when authentication is implemented.
/// - Add pagination to GET collection when volume grows beyond practical limits.
/// </remarks>
/// <param name="commentService">The injected asynchronous comment business logic service.</param>
[Route("api/[controller]")]
[ApiController]
public class CommentController(ICommentService commentService) : ControllerBase
    {
    /// <summary>
    /// Retrieves all comments.
    /// </summary>
    /// <returns>200 OK with collection payload.</returns>
    [HttpGet]
    public async Task<IActionResult> Get()
        {
        var comments = await commentService.GetCommentsAsync();
        return Ok(comments);
        }

    /// <summary>
    /// Retrieves a specific comment by identifier.
    /// </summary>
    /// <param name="id">Comment identifier.</param>
    /// <returns>200 OK with resource OR 404 if not found.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
        {
        var comment = await commentService.GetByIdAsync(id);
        if (comment == null)
            {
            return NotFound($"Comment with id: {id} not found");
            }

        return Ok(comment);
        }

    /// <summary>
    /// Creates a new comment.
    /// </summary>
    /// <param name="request">Incoming request payload.</param>
    /// <returns>201 Created with location header reference.</returns>
    [HttpPost]
    public async Task<IActionResult> Post(CommentRequestDto request)
        {
        var author = User.FindFirstValue(ClaimTypes.Name) ?? "Anonymous/System User"; // Placeholder until auth added

        await commentService.AddAsync(request, author);

        // NOTE: The created resource ID would be returned from service in a richer implementation.
        return CreatedAtAction(nameof(Get), new { id = Guid.Empty /* Replace with actual ID when returned */ }, "Comment added successfully");
        }

    /// <summary>
    /// Updates an existing comment.
    /// </summary>
    /// <param name="id">Comment identifier.</param>
    /// <param name="request">Updated values payload.</param>
    /// <returns>200 OK if updated OR 404 when not found.</returns>
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
    /// Deletes a comment permanently.
    /// </summary>
    /// <param name="id">Comment identifier.</param>
    /// <returns>204 No Content when deletion succeeds OR 404 if target absent.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
        {
        var deleted = await commentService.DeleteAsync(id);
        if (!deleted)
            {
            return NotFound($"Comment with id: {id} not found");
            }

        return NoContent();
        }
    }