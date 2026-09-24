using System.ComponentModel.DataAnnotations;
using TaskFlowApi.Entities;

namespace TaskFlowApi.DTOs;

public class CreateTaskDto : IValidatableObject
{
    [Required(ErrorMessage = "Заголовок задачи обязателен")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "Заголовок должен быть от 5 до 200 символов")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    public TaskItemStatus Status { get; set; } = TaskItemStatus.ToDo;

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    [Required]
    public int ProjectId { get; set; }

    public int? AssignedToId { get; set; }

    public DateTime? DueDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
    {
        if (DueDate.HasValue && DueDate.Value <= DateTime.UtcNow)
        {
            yield return new ValidationResult("DueDate должна быть в будущем", new[] { nameof(DueDate) });
        }
    }
}