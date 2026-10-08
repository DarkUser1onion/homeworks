using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureTodo.Data;
using SecureTodo.DTOs;
using SecureTodo.Models;

namespace SecureTodo.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class TasksApiController : ControllerBase
{
    private readonly AppDbContext _db;
    public TasksApiController(AppDbContext db) => _db = db;

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

    private bool IsAdmin => User.IsInRole("Admin");

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskDto>>> GetAll()
    {
        var query = _db.Tasks.Include(t => t.User).AsQueryable();

        if (!IsAdmin)
            query = query.Where(t => t.UserId == CurrentUserId);

        var items = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
        return Ok(items.Select(ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskDto>> GetById(int id)
    {
        var task = await _db.Tasks.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);
        if (task == null) return NotFound();

        if (!IsAdmin && task.UserId != CurrentUserId) return Forbid();

        return Ok(ToDto(task));
    }

    [HttpPost]
    public async Task<ActionResult<TaskDto>> Create([FromBody] TaskCreateDto input)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var task = new TaskItem
        {
            Title = input.Title,
            Description = input.Description,
            UserId = CurrentUserId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();

        await _db.Entry(task).Reference(t => t.User).LoadAsync();
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, ToDto(task));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TaskUpdateDto input)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        if (!IsAdmin && task.UserId != CurrentUserId) return Forbid();

        task.Title = input.Title;
        task.Description = input.Description;
        task.IsCompleted = input.IsCompleted;
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        if (!IsAdmin && task.UserId != CurrentUserId) return Forbid();

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static TaskDto ToDto(TaskItem t) => new()
    {
        Id = t.Id,
        Title = t.Title,
        Description = t.Description,
        IsCompleted = t.IsCompleted,
        CreatedAt = t.CreatedAt,
        UserId = t.UserId,
        UserEmail = t.User?.Email
    };
}