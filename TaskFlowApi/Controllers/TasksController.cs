using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.DTOs;
using TaskFlowApi.Entities;
using TaskFlowApi.Mapping;

namespace TaskFlowApi.Controllers;

/// <summary>Управление задачами (v1).</summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _db;
    public TasksController(AppDbContext db) => _db = db;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetAll()
    {
        var items = await _db.Tasks.Include(t => t.AssignedTo).ToListAsync();
        return Ok(items.Select(t => t.ToDto()));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskItemDto>> GetById(int id)
    {
        var t = await _db.Tasks.Include(x => x.AssignedTo).FirstOrDefaultAsync(x => x.Id == id);
        return t is null ? NotFound() : Ok(t.ToDto());
    }

    [HttpPost]
    [ProducesResponseType(typeof(TaskItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskItemDto>> Create([FromBody] CreateTaskDto input)
    {
        if (!await _db.Projects.AnyAsync(p => p.Id == input.ProjectId))
            return BadRequest(new { message = "Проект не найден" });

        var t = new TaskItem
        {
            Title = input.Title,
            Description = input.Description,
            Status = input.Status,
            Priority = input.Priority,
            ProjectId = input.ProjectId,
            AssignedToId = input.AssignedToId,
            DueDate = input.DueDate
        };
        _db.Tasks.Add(t);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = t.Id, version = "1" }, t.ToDto());
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto input)
    {
        var t = await _db.Tasks.FindAsync(id);
        if (t is null) return NotFound();
        if (!input.Status.HasValue) return BadRequest(new { message = "Status обязателен" });

        t.Title = input.Title;
        t.Description = input.Description;
        t.Status = input.Status.Value;
        t.Priority = input.Priority;
        t.AssignedToId = input.AssignedToId;
        t.DueDate = input.DueDate;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await _db.Tasks.FindAsync(id);
        if (t is null) return NotFound();

        _db.Tasks.Remove(t);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}