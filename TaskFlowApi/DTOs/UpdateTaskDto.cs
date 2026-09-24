using System.ComponentModel.DataAnnotations;
using TaskFlowApi.Entities;

namespace TaskFlowApi.DTOs;

public class UpdateTaskDto : IValidatableObject
{
    [Required(ErrorMessage = "Заголовок задачи обязателен")]
    [StringLength(200, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Status обязателен при PUT")]
    public TaskItemStatus? Status { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public int? AssignedToId { get; set; }

    public DateTime? DueDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
    {
        if (!Status.HasValue)
        {
            yield return new ValidationResult("Status обязателен при полной замене", new[] { nameof(Status) });
        }
        if (DueDate.HasValue && DueDate.Value <= DateTime.UtcNow)
        {
            yield return new ValidationResult("DueDate должна быть в будущем", new[] { nameof(DueDate) });
        }
    }
}