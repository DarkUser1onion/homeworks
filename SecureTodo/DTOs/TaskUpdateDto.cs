using System.ComponentModel.DataAnnotations;

namespace SecureTodo.DTOs;

public class TaskUpdateDto
{
    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    public bool IsCompleted { get; set; }
}