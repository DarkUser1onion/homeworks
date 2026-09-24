using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.DTOs;
using TaskFlowApi.Entities;
using TaskFlowApi.Mapping;

namespace TaskFlowApi.Controllers;

/// <summary>Комментарии задач (вложенный ресурс).</summary>
[ApiController]
[Route("api")]
[Produces("application/json")]
public class CommentsController : ControllerBase
{
    private readonly AppDbContext _db;
    public CommentsController(AppDbContext db) => _db = db;

    /// <summary>Список комментариев задачи.</summary>
    [HttpGet("tasks/{taskId:int}/comments")]
    [ProducesResponseType(typeof(IEnumerable<CommentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetForTask(int taskId)
    {
        if (!await _db.Tasks.AnyAsync(t => t.Id == taskId))
            return NotFound(new { message = "Задача не найдена" });

        var comments = await _db.Comments
            .Include(c => c.Author)
            .Where(c => c.TaskItemId == taskId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        return Ok(comments.Select(c => c.ToDto()));
    }

    /// <summary>Добавить комментарий к задаче.</summary>
    [HttpPost("tasks/{taskId:int}/comments")]
    [ProducesResponseType(typeof(CommentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommentDto>> Create(int taskId, [FromBody] CreateCommentDto input)
    {
        if (!await _db.Tasks.AnyAsync(t => t.Id == taskId))
            return NotFound(new { message = "Задача не найдена" });

        if (!await _db.Users.AnyAsync(u => u.Id == input.AuthorId))
            return BadRequest(new { message = "Автор не найден" });

        var c = new Comment
        {
            TaskItemId = taskId,
            AuthorId = input.AuthorId,
            Content = input.Content
        };
        _db.Comments.Add(c);
        await _db.SaveChangesAsync();

        // Подгружаем автора для DTO
        await _db.Entry(c).Reference(x => x.Author).LoadAsync();

        return CreatedAtAction(nameof(GetForTask), new { taskId }, c.ToDto());
    }

    /// <summary>Удалить комментарий.</summary>
    [HttpDelete("comments/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await _db.Comments.FindAsync(id);
        if (c is null) return NotFound();

        _db.Comments.Remove(c);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}