using System.ComponentModel.DataAnnotations;

namespace TaskFlowApi.Entities;

/// <summary>Комментарий к задаче.</summary>
public class Comment
{
    public int Id { get; set; }

    [Required]
    public int TaskItemId { get; set; }

    [Required]
    public int AuthorId { get; set; }

    [Required(ErrorMessage = "Текст комментария обязателен")]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "Текст должен быть от 1 до 500 символов")]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Навигационные свойства
    public TaskItem TaskItem { get; set; } = null!;
    public AppUser Author { get; set; } = null!;
}