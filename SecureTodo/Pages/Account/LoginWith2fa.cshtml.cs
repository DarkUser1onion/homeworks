using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecureTodo.Models;

namespace SecureTodo.Pages.Account;

public class LoginWith2faModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;

    public LoginWith2faModel(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public bool RememberMe { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(7, MinimumLength = 6)]
        [DataType(DataType.Text)]
        public string Code { get; set; } = string.Empty;

        public bool RememberMachine { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(bool rememberMe = false)
    {
        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null) return RedirectToPage("/Account/Login");

        RememberMe = rememberMe;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(bool rememberMe = false)
    {
        if (!ModelState.IsValid) return Page();

        var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
        if (user == null) return RedirectToPage("/Account/Login");

        var code = Input.Code.Replace(" ", "").Replace("-", "");

        var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(
            code, rememberMe, Input.RememberMachine);

        if (result.Succeeded) return RedirectToPage("/Index");

        if (result.IsLockedOut)
        {
            ModelState.AddModelError("", "Аккаунт заблокирован.");
            return Page();
        }

        ModelState.AddModelError("", "Неверный код.");
        return Page();
    }
}