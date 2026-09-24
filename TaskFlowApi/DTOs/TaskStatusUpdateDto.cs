using System.ComponentModel.DataAnnotations;
using TaskFlowApi.Entities;

namespace TaskFlowApi.DTOs;

public class TaskStatusUpdateDto
{
    [Required]
    public TaskItemStatus Status { get; set; }
}