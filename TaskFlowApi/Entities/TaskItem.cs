using System.ComponentModel.DataAnnotations;

namespace TaskFlowApi.Entities;

public enum TaskStatus { ToDo, InProgress, Done }

public class TaskItem : IValidatableObject
{
    public int Id { get; set; }

    [Required]
    public int ProjectId { get; set; }

    [Required(ErrorMessage = "Заголовок задачи обязателен")]
    [StringLength(200, MinimumLength = 5,
        ErrorMessage = "Заголовок должен быть от 5 до 200 символов")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.ToDo;

    public int? AssignedToId { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
    {
        if (DueDate.HasValue && DueDate.Value <= DateTime.UtcNow)
        {
            yield return new ValidationResult(
                "DueDate должна быть в будущем",
                new[] { nameof(DueDate) });
        }
    }
}