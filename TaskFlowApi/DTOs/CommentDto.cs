namespace TaskFlowApi.DTOs;

public class CommentDto
{
    public int Id { get; set; }
    public int TaskItemId { get; set; }
    public string AuthorUsername { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}