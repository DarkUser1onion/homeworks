namespace TaskBoard.Services;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public bool IsDone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public interface ITaskService
{
    List<TaskItem> GetAll();
    TaskItem? GetById(int id);
    TaskItem Add(string title, string? description);
    bool MarkDone(int id);
    bool Delete(int id);
}

public class CreateTaskRequest
{
    public string Title { get; set; } = "";
    public string? Description { get; set; }
}