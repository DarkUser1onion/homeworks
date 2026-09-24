using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Data;
using TaskFlowApi.DTOs;
using TaskFlowApi.Entities;
using TaskFlowApi.Exceptions;
using TaskFlowApi.Services;

namespace TaskFlowApi.Controllers;

/// <summary>Управление задачами (v2) - Priority, пагинация, идемпотентность.</summary>
[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/tasks")]
[Produces("application/json")]
public class TasksV2Controller : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IdempotencyService _idem;

    public TasksV2Controller(AppDbContext db, IdempotencyService idem)
    {
        _db = db;
        _idem = idem;
    }

    /// <summary>Список задач с пагинацией, фильтрацией, сортировкой и поиском.</summary>
    /// <param name="page">Номер страницы (по умолчанию 1).</param>
    /// <param name="pageSize">Размер страницы (по умолчанию 10, максимум 50).</param>
    /// <param name="status">Фильтр по статусу.</param>
    /// <param name="priority">Фильтр по приоритету.</param>
    /// <param name="sortBy">Поле сортировки: title, dueDate, createdAt.</param>
    /// <param name="sortDir">Направление: asc / desc.</param>
    /// <param name="search">Поиск по Title (содержит).</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TaskItemV2Dto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<TaskItemV2Dto>>> GetPaged(
        int page = 1,
        int pageSize = 10,
        string? status = null,
        string? priority = null,
        string? sortBy = "createdAt",
        string? sortDir = "asc",
        string? search = null)
    {
        if (pageSize > 50)
            return BadRequest(new ProblemDetails
            {
                Title = "Validation error",
                Status = 400,
                Detail = "pageSize не может быть больше 50"
            });
        if (page < 1) page = 1;

        var query = _db.Tasks.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<TaskItemStatus>(status, true, out var st))
        {
            query = query.Where(t => t.Status == st);
        }
        if (!string.IsNullOrWhiteSpace(priority) &&
            Enum.TryParse<TaskPriority>(priority, true, out var pr))
        {
            query = query.Where(t => t.Priority == pr);
        }
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.ToLower();
            query = query.Where(t => t.Title.ToLower().Contains(s));
        }

        query = (sortBy?.ToLower(), sortDir?.ToLower()) switch
        {
            ("title", "desc") => query.OrderByDescending(t => t.Title),
            ("title", _) => query.OrderBy(t => t.Title),
            ("duedate", "desc") => query.OrderByDescending(t => t.DueDate),
            ("duedate", _) => query.OrderBy(t => t.DueDate),
            ("createdat", "desc") => query.OrderByDescending(t => t.CreatedAt),
            _ => query.OrderBy(t => t.CreatedAt)
        };

        var total = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = new PagedResult<TaskItemV2Dto>
        {
            Items = items.Select(t => new TaskItemV2Dto
            {
                Id = t.Id,
                Title = t.Title,
                Status = t.Status,
                Priority = t.Priority,
                ProjectId = t.ProjectId,
                DueDate = t.DueDate,
                CreatedAt = t.CreatedAt
            }).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };

        return Ok(result);
    }

    /// <summary>Задача v2 по ID.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TaskItemV2Dto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskItemV2Dto>> GetById(int id)
    {
        var t = await _db.Tasks.FindAsync(id)
            ?? throw new NotFoundException($"Задача с id={id} не найдена");

        return Ok(new TaskItemV2Dto
        {
            Id = t.Id,
            Title = t.Title,
            Status = t.Status,
            Priority = t.Priority,
            ProjectId = t.ProjectId,
            DueDate = t.DueDate,
            CreatedAt = t.CreatedAt
        });
    }

    /// <summary>Создать задачу с защитой от дубликатов через X-Idempotency-Key.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TaskItemV2Dto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto input)
    {
        if (!Request.Headers.TryGetValue("X-Idempotency-Key", out var keyValues)
            || string.IsNullOrWhiteSpace(keyValues.ToString()))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Validation error",
                Status = 400,
                Detail = "Заголовок X-Idempotency-Key обязателен"
            });
        }

        var key = keyValues.ToString();
        var hash = _idem.ComputeHash(input);

        var existing = await _idem.FindAsync(key);
        if (existing != null)
        {
            if (existing.RequestBodyHash != hash)
            {
                return Conflict(new ProblemDetails
                {
                    Title = "Conflict",
                    Status = 409,
                    Detail = "Idempotency key reused with different body"
                });
            }

            return new ContentResult
            {
                Content = existing.ResponseBody,
                ContentType = "application/json",
                StatusCode = existing.StatusCode
            };
        }

        if (!await _db.Projects.AnyAsync(p => p.Id == input.ProjectId))
            return BadRequest(new ProblemDetails { Title = "Validation error", Status = 400, Detail = "Проект не найден" });

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

        var dto = new TaskItemV2Dto
        {
            Id = t.Id,
            Title = t.Title,
            Status = t.Status,
            Priority = t.Priority,
            ProjectId = t.ProjectId,
            DueDate = t.DueDate,
            CreatedAt = t.CreatedAt
        };

        await _idem.SaveAsync(key, hash, _idem.Serialize(dto), 201);

        return CreatedAtAction(nameof(GetById), new { id = t.Id, version = "2" }, dto);
    }

    /// <summary>Сменить статус задачи. Нельзя закрыть без комментариев.</summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ChangeStatus(int id, [FromBody] TaskStatusUpdateDto input)
    {
        var t = await _db.Tasks.FindAsync(id)
            ?? throw new NotFoundException($"Задача с id={id} не найдена");

        if (input.Status == TaskItemStatus.Done)
        {
            var hasComments = await _db.Comments.AnyAsync(c => c.TaskItemId == id);
            if (!hasComments)
                throw new BusinessRuleException("Нельзя завершить задачу без комментариев");
        }

        t.Status = input.Status;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}