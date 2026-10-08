using System.ComponentModel.DataAnnotations;

namespace SecureTodo.DTOs;

public class RefreshRequestDto
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}