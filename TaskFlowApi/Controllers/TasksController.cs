using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.Entities;

namespace TaskFlowApi.Controllers;

/// <summary>Управление задачами.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TasksController : ControllerBase
{
    private static readonly List<TaskItem> _tasks = new();
    private static int _nextId = 1;

    /// <summary>Список всех задач.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskItem>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<TaskItem>> GetAll()
        => Ok(_tasks);

    /// <summary>Получить задачу по ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TaskItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TaskItem> GetById(int id)
    {
        var t = _tasks.FirstOrDefault(x => x.Id == id);
        return t is null ? NotFound() : Ok(t);
    }

    /// <summary>Создать задачу. Если Status не указан - по умолчанию ToDo.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TaskItem), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<TaskItem> Create([FromBody] TaskItem input)
    {
        // Если Status не пришёл - enum по умолчанию уже ToDo (см. TaskItem).
        input.Id = _nextId++;
        input.CreatedAt = DateTime.UtcNow;
        _tasks.Add(input);

        return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
    }

    /// <summary>Полностью обновить задачу. Status обязателен при PUT.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] TaskItem input)
    {

        var t = _tasks.FirstOrDefault(x => x.Id == id);
        if (t is null) return NotFound();

        if (!ModelState.IsValid) return BadRequest(ModelState);

        t.Title = input.Title;
        t.Description = input.Description;
        t.Status = input.Status;
        t.ProjectId = input.ProjectId;
        t.AssignedToId = input.AssignedToId;
        t.DueDate = input.DueDate;

        return NoContent();
    }

    /// <summary>Удалить задачу.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var t = _tasks.FirstOrDefault(x => x.Id == id);
        if (t is null) return NotFound();

        _tasks.Remove(t);
        return NoContent();
    }
}