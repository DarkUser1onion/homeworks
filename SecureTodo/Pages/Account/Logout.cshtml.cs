using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecureTodo.Models;

namespace SecureTodo.Pages.Account;

public class LogoutModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;
    public LogoutModel(SignInManager<AppUser> signInManager) => _signInManager = signInManager;

    public IActionResult OnGet() => RedirectToPage("/Index");

    public async Task<IActionResult> OnPostAsync()
    {
        await _signInManager.SignOutAsync();
        return RedirectToPage("/Index");
    }
}