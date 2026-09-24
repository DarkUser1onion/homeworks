using TaskFlowApi.Entities;

namespace TaskFlowApi.DTOs;

public class TaskItemV2Dto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public TaskItemStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public int ProjectId { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
}