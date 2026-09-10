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

    public IActionResult Index(string? status = "all")
    {
        var all = _service.GetAll();

        var filtered = status switch
        {
            "done"   => all.Where(t => t.IsDone).ToList(),
            "active" => all.Where(t => !t.IsDone).ToList(),
            _        => all
        };

        ViewBag.Total = all.Count;
        ViewBag.Done  = all.Count(t => t.IsDone);

        ViewBag.Status = (status is "done" or "active") ? status : "all";

        return View(filtered);
    }

    public IActionResult Details(int id, string? status = "all")
    {
        var task = _service.GetById(id);
        if (task == null) return NotFound();

        ViewBag.Status = (status is "done" or "active") ? status : "all";
        return View(task);
    }

    public IActionResult Create(string? status = "all")
    {
        ViewBag.Status = (status is "done" or "active") ? status : "all";
        return View();
    }

    [HttpPost]
    public IActionResult Create(string title, string? description, string? status = "all")
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            ModelState.AddModelError("title", "Заголовок обязателен");
            ViewBag.Status = (status is "done" or "active") ? status : "all";
            return View();
        }

        _service.Add(title, description);
        return RedirectToAction(nameof(Index), new { status });
    }

    [HttpPost]
    public IActionResult Done(int id, string? status = "all")
    {
        _service.MarkDone(id);
        return RedirectToAction(nameof(Index), new { status });
    }

    [HttpPost]
    public IActionResult Delete(int id, string? status = "all")
    {
        _service.Delete(id);
        return RedirectToAction(nameof(Index), new { status });
    }

    [HttpGet("tasks/api/list")]
    public IActionResult ApiList() => Json(_service.GetAll());

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