using System.ComponentModel.DataAnnotations;

namespace TaskFlowApi.Entities;

public class Project
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название проекта обязательно")]
    [StringLength(100, MinimumLength = 3,
        ErrorMessage = "Название должно быть от 3 до 100 символов")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}