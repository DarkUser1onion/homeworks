namespace TaskFlowApi.Entities;

/// <summary>Проект - верхнеуровневая сущность, содержит задачи.</summary>
public class Project
{
    public int Id { get; set; }

    /// <summary>Название проекта.</summary>
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}