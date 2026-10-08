using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecureTodo.Models;

namespace SecureTodo.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;

    public LoginModel(SignInManager<AppUser> signInManager) => _signInManager = signInManager;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var result = await _signInManager.PasswordSignInAsync(
            Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded) return RedirectToPage("/Index");

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Аккаунт заблокирован на 15 минут из-за неудачных попыток.");
            return Page();
        }

        ModelState.AddModelError(string.Empty, "Неверный email или пароль.");
        return Page();
    }
}