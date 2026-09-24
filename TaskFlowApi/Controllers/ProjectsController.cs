using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.Entities;

namespace TaskFlowApi.Controllers;

/// <summary>Управление проектами.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProjectsController : ControllerBase
{
    // Пока - статический список. На уровне 2 заменим на EF Core.
    private static readonly List<Project> _projects = new();
    private static int _nextId = 1;

    /// <summary>Получить список всех проектов.</summary>
    /// <response code="200">Список проектов (может быть пустым).</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Project>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Project>> GetAll()
        => Ok(_projects);

    /// <summary>Получить проект по идентификатору.</summary>
    /// <param name="id">ID проекта.</param>
    /// <response code="200">Проект найден.</response>
    /// <response code="404">Проект не найден.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Project), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Project> GetById(int id)
    {
        var p = _projects.FirstOrDefault(x => x.Id == id);
        return p is null ? NotFound() : Ok(p);
    }

    /// <summary>Создать новый проект.</summary>
    /// <response code="201">Проект создан, в заголовке Location - ссылка на него.</response>
    /// <response code="400">Ошибка валидации.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Project), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Project> Create([FromBody] Project input)
    {
        input.Id = _nextId++;
        input.CreatedAt = DateTime.UtcNow;
        _projects.Add(input);

        return CreatedAtAction(nameof(GetById), new { id = input.Id }, input);
    }

    /// <summary>Полностью обновить проект.</summary>
    /// <response code="204">Обновлено успешно.</response>
    /// <response code="404">Проект не найден.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] Project input)
    {
        var p = _projects.FirstOrDefault(x => x.Id == id);
        if (p is null) return NotFound();

        p.Name = input.Name;
        p.Description = input.Description;

        return NoContent();
    }

    /// <summary>Удалить проект.</summary>
    /// <response code="204">Удалено успешно.</response>
    /// <response code="404">Проект не найден.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var p = _projects.FirstOrDefault(x => x.Id == id);
        if (p is null) return NotFound();

        _projects.Remove(p);
        return NoContent();
    }
}