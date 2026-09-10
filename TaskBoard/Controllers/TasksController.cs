using Microsoft.AspNetCore.Mvc;
using TaskBoard.Services;

namespace TaskBoard.Controllers;

public class TasksController : Controller
{
    private readonly ITaskService _service;

    public TasksController(ITaskService service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        var tasks = _service.GetAll();

        ViewBag.Total = tasks.Count;
        ViewBag.Done = tasks.Count(t => t.IsDone);

        return View(tasks);
    }

    public IActionResult Details(int id)
    {
        var task = _service.GetById(id);
        if (task == null) return NotFound();
        return View(task);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(string title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            ModelState.AddModelError("title", "Заголовок обязателен");
            return View();
        }

        _service.Add(title, description);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Done(int id)
    {
        _service.MarkDone(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        _service.Delete(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("tasks/api/list")]
    public IActionResult ApiList()
    {
        var tasks = _service.GetAll();
        return Json(tasks);
    }

    [HttpGet("tasks/api/{id:int}")]
    public IActionResult ApiGet(int id)
    {
        var task = _service.GetById(id);
        if (task == null) return NotFound();
        return Json(task);
    }

    [HttpPost("tasks/api/create")]
    public IActionResult ApiCreate([FromBody] CreateTaskRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new { error = "Заголовок обязателен" });

        var task = _service.Add(request.Title, request.Description);
        return CreatedAtAction(nameof(ApiGet), new { id = task.Id }, task);
    }
}