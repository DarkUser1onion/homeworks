using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecureTodo.Data;
using SecureTodo.Models;
using Microsoft.AspNetCore.Mvc;

namespace SecureTodo.Pages.Tasks;

[Authorize]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _userManager;
    private readonly IAuthorizationService _auth;

    public IndexModel(AppDbContext db, UserManager<AppUser> userManager, IAuthorizationService auth)
    {
        _db = db;
        _userManager = userManager;
        _auth = auth;
    }

    public List<TaskItem> Tasks { get; set; } = new();
    public string CurrentUserId { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        CurrentUserId = _userManager.GetUserId(User) ?? "";

        Tasks = await _db.Tasks
            .Include(t => t.User)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostToggleCompleteAsync(int id)
    {
        var userId = _userManager.GetUserId(User) ?? "";
        var task = await _db.Tasks.FindAsync(id);

        if (task == null) return NotFound();

        if (!User.IsInRole("Admin") && task.UserId != userId)
            return Forbid();

        task.IsCompleted = !task.IsCompleted;
        await _db.SaveChangesAsync();

        return new JsonResult(new { isCompleted = task.IsCompleted });
    }

    public async Task<AuthorizationResult> CanEditAsync(TaskItem task)
    {
        return await _auth.AuthorizeAsync(User, task, "CanEditTask");
    }
}