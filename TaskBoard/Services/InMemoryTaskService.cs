namespace TaskBoard.Services;

public class InMemoryTaskService : ITaskService
{
    private readonly List<TaskItem> _tasks = new();
    private int _nextId = 1;

    public List<TaskItem> GetAll() => _tasks.ToList();

    public TaskItem? GetById(int id) => _tasks.FirstOrDefault(t => t.Id == id);

    public TaskItem Add(string title, string? description)
    {
        var task = new TaskItem
        {
            Id = _nextId++,
            Title = title,
            Description = description,
            IsDone = false
        };
        _tasks.Add(task);
        return task;
    }

    public bool MarkDone(int id)
    {
        var task = GetById(id);
        if (task == null) return false;
        task.IsDone = true;
        return true;
    }

    public bool Delete(int id)
    {
        var task = GetById(id);
        if (task == null) return false;
        _tasks.Remove(task);
        return true;
    }
}