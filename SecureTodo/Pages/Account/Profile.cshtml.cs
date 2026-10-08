using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecureTodo.Models;

namespace SecureTodo.Pages.Account;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;

    public ProfileModel(UserManager<AppUser> userManager) => _userManager = userManager;

    public string Email { get; set; } = "";
    public string? FullName { get; set; }
    public bool Is2faEnabled { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Account/Login");

        Email = user.Email ?? "";
        FullName = user.FullName;
        Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostDisable2faAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Account/Login");

        await _userManager.SetTwoFactorEnabledAsync(user, false);
        await _userManager.ResetAuthenticatorKeyAsync(user);

        return RedirectToPage();
    }
}