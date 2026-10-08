using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecureTodo.DTOs;
using SecureTodo.Models;
using SecureTodo.Services;

namespace SecureTodo.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthApiController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly JwtService _jwt;

    public AuthApiController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        JwtService jwt)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwt = jwt;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto input)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(input.Email);
        if (user == null)
            return Unauthorized(new { error = "Неверный email или пароль" });

        var result = await _signInManager.CheckPasswordSignInAsync(
            user, input.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
            return Unauthorized(new { error = "Аккаунт заблокирован" });

        if (!result.Succeeded)
            return Unauthorized(new { error = "Неверный email или пароль" });

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expires) = _jwt.GenerateToken(user, roles);

        return Ok(new
        {
            token,
            expiresAt = expires,
            email = user.Email,
            roles
        });
    }
}