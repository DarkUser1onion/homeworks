namespace TaskFlowApi.Entities;

public class AppUser
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public List<TaskItem> AssignedTasks { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
}