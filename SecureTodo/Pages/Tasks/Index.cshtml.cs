using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecureTodo.Data;
using SecureTodo.Models;

namespace SecureTodo.Pages.Tasks;

[Authorize]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly UserManager<AppUser> _userManager;

    public IndexModel(AppDbContext db, UserManager<AppUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public List<TaskItem> Tasks { get; set; } = new();
    public string CurrentUserId { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        CurrentUserId = _userManager.GetUserId(User) ?? "";

        var query = _db.Tasks.Include(t => t.User).AsQueryable();
        if (!User.IsInRole("Admin"))
        {
            query = query.Where(t => t.UserId == CurrentUserId);
        }

        Tasks = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
    }
}