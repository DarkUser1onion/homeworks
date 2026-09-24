using TaskFlowApi.Entities;

namespace TaskFlowApi.DTOs;

public class TaskItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; }
    public int ProjectId { get; set; }
    public int? AssignedToId { get; set; }
    public string? AssignedToUsername { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
}