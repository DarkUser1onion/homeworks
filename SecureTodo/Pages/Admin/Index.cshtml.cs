using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecureTodo.Models;

namespace SecureTodo.Pages.Admin;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;

    public IndexModel(UserManager<AppUser> userManager) => _userManager = userManager;

    public List<UserRow> Users { get; set; } = new();

    public class UserRow
    {
        public string Email { get; set; } = "";
        public string? FullName { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public async Task OnGetAsync()
    {
        foreach (var user in _userManager.Users.ToList())
        {
            var roles = await _userManager.GetRolesAsync(user);
            Users.Add(new UserRow
            {
                Email = user.Email ?? "",
                FullName = user.FullName,
                Roles = roles.ToList()
            });
        }
    }
}