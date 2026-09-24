using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.DTOs;
using TaskFlowApi.Entities;
using TaskFlowApi.Mapping;

namespace TaskFlowApi.Controllers;

/// <summary>Управление проектами.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ProjectsController(AppDbContext db) => _db = db;

    /// <summary>Список всех проектов.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll()
    {
        var items = await _db.Projects
            .Include(p => p.Tasks)
            .OrderBy(p => p.Id)
            .ToListAsync();

        return Ok(items.Select(p => p.ToDto()));
    }

    /// <summary>Проект по ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> GetById(int id)
    {
        var p = await _db.Projects.Include(x => x.Tasks).FirstOrDefaultAsync(x => x.Id == id);
        return p is null ? NotFound() : Ok(p.ToDto());
    }

    /// <summary>Создать проект.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectDto input)
    {
        var p = new Project
        {
            Name = input.Name,
            Description = input.Description,
            CreatedAt = DateTime.UtcNow
        };
        _db.Projects.Add(p);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = p.Id }, p.ToDto());
    }

    /// <summary>Обновить проект.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectDto input)
    {
        var p = await _db.Projects.FindAsync(id);
        if (p is null) return NotFound();

        p.Name = input.Name;
        p.Description = input.Description;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Удалить проект (задачи удалятся каскадно).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _db.Projects.FindAsync(id);
        if (p is null) return NotFound();

        _db.Projects.Remove(p);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}