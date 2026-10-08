namespace SecureTodo.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsCompleted { get; set; }

    // Внешний ключ
    public string UserId { get; set; } = string.Empty;
    public AppUser? User { get; set; }
}