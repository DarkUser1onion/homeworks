using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecureTodo.Models;
using Microsoft.AspNetCore.Mvc;

namespace SecureTodo.Pages.Admin;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;

    public IndexModel(UserManager<AppUser> userManager) => _userManager = userManager;

    public List<UserRow> Users { get; set; } = new();

    public class UserRow
    {
        public string Id { get; set; } = "";
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
                Id = user.Id,
                Email = user.Email ?? "",
                FullName = user.FullName,
                Roles = roles.ToList()
            });
        }
    }

    public async Task<IActionResult> OnPostChangeRoleAsync(string userId, string newRole)
    {
        var target = await _userManager.FindByIdAsync(userId);
        if (target == null) return NotFound();

        var currentUserId = _userManager.GetUserId(User);
        if (target.Id == currentUserId && newRole != "Admin")
        {
            TempData["Error"] = "Нельзя снять роль Admin с самого себя.";
            return RedirectToPage();
        }

        var oldRoles = await _userManager.GetRolesAsync(target);
        await _userManager.RemoveFromRolesAsync(target, oldRoles);

        await _userManager.AddToRoleAsync(target, newRole);

        await _userManager.UpdateSecurityStampAsync(target);

        TempData["Success"] = $"Роль пользователя {target.Email} изменена на {newRole}.";
        return RedirectToPage();
    }
}