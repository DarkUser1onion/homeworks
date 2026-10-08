using Microsoft.AspNetCore.Identity;

namespace SecureTodo.Models;

public class AppUser : IdentityUser
{
    public string? FullName { get; set; }
}