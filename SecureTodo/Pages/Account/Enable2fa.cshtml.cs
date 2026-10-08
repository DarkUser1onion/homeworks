using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SecureTodo.Models;

namespace SecureTodo.Pages.Account;

[Authorize]
public class Enable2faModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _config;

    public Enable2faModel(UserManager<AppUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    public string SharedKey { get; set; } = "";
    public string QrCodeUrl { get; set; } = "";
    public bool Is2faEnabled { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Account/Login");

        Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
        if (Is2faEnabled) return Page();

        var key = await _userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(key))
        {
            await _userManager.ResetAuthenticatorKeyAsync(user);
            key = await _userManager.GetAuthenticatorKeyAsync(user);
        }

        SharedKey = FormatKey(key!);

        var issuer = "SecureTodo";
        var email = user.Email ?? user.UserName ?? "";
        QrCodeUrl = $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(email)}" +
                    $"?secret={key}&issuer={Uri.EscapeDataString(issuer)}&digits=6";

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string code)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToPage("/Account/Login");

        if (string.IsNullOrWhiteSpace(code))
        {
            ModelState.AddModelError("", "Введите код");
        }
        else
        {
            var key = await _userManager.GetAuthenticatorKeyAsync(user);
            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                code.Replace(" ", "").Replace("-", ""));

            if (isValid)
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                return RedirectToPage("/Account/Profile");
            }

            ModelState.AddModelError("", "Неверный код. Попробуйте ещё раз.");
        }

        return await OnGetAsync();
    }

    private static string FormatKey(string key)
    {
        var result = new System.Text.StringBuilder();
        for (int i = 0; i < key.Length; i++)
        {
            if (i > 0 && i % 4 == 0) result.Append(' ');
            result.Append(key[i]);
        }
        return result.ToString().ToLowerInvariant();
    }
}