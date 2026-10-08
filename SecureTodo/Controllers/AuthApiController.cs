using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
    private readonly RefreshTokenService _refresh;

    public AuthApiController(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        JwtService jwt,
        RefreshTokenService refresh)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwt = jwt;
        _refresh = refresh;
    }

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

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

        return Ok(await BuildAuthResponseAsync(user));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto input)
    {
        var oldToken = await _refresh.GetActiveAsync(input.RefreshToken);
        if (oldToken == null)
            return Unauthorized(new { error = "Refresh-токен недействителен или отозван" });

        var user = await _userManager.FindByIdAsync(oldToken.UserId);
        if (user == null)
            return Unauthorized(new { error = "Пользователь не найден" });

        await _refresh.RevokeAsync(oldToken);
        return Ok(await BuildAuthResponseAsync(user));
    }

    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequestDto input)
    {
        var token = await _refresh.GetActiveAsync(input.RefreshToken);
        if (token == null) return Ok(new { revoked = false });

        await _refresh.RevokeAsync(token);
        return Ok(new { revoked = true });
    }

    [HttpPost("logout-all")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> LogoutAll()
    {
        var userId = CurrentUserId;
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        await _refresh.RevokeAllAsync(userId);
        return Ok(new { revokedAll = true });
    }

    private async Task<AuthResponseDto> BuildAuthResponseAsync(AppUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var (accessToken, accessExpires) = _jwt.GenerateToken(user, roles);
        var refreshToken = await _refresh.CreateAsync(user);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            AccessExpiresAt = accessExpires,
            RefreshExpiresAt = refreshToken.ExpiresAt,
            Email = user.Email ?? "",
            Roles = roles
        };
    }
}