using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;

namespace TaskFlowApi.Controllers;

/// <summary>Управление задачами (v2) - с Priority, без Description в ответе.</summary>
[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/tasks")]
[Produces("application/json")]
public class TasksV2Controller : ControllerBase
{
    private readonly AppDbContext _db;
    public TasksV2Controller(AppDbContext db) => _db = db;

    /// <summary>Список задач v2 - есть priority, нет description.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var items = await _db.Tasks.ToListAsync();
        return Ok(items.Select(t => new
        {
            id = t.Id,
            title = t.Title,
            status = t.Status.ToString(),
            priority = t.Priority.ToString(),
            projectId = t.ProjectId,
            dueDate = t.DueDate,
            createdAt = t.CreatedAt
        }));
    }

    /// <summary>Задача v2 - есть priority, нет description.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var t = await _db.Tasks.FindAsync(id);
        if (t is null) return NotFound();

        return Ok(new
        {
            id = t.Id,
            title = t.Title,
            status = t.Status.ToString(),
            priority = t.Priority.ToString(),
            projectId = t.ProjectId,
            dueDate = t.DueDate,
            createdAt = t.CreatedAt
        });
    }
}