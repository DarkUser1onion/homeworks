using System.ComponentModel.DataAnnotations;

namespace TaskFlowApi.DTOs;

public class CreateCommentDto
{
    [Required(ErrorMessage = "Текст комментария обязателен")]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "Текст должен быть от 1 до 500 символов")]
    public string Content { get; set; } = string.Empty;

    [Required]
    public int AuthorId { get; set; }
}