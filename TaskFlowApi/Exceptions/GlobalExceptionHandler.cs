using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TaskFlowApi.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext ctx, Exception ex, CancellationToken ct)
    {
        _logger.LogError(ex, "Необработанное исключение: {Message}", ex.Message);

        var (status, title, detail) = ex switch
        {
            NotFoundException nf => (StatusCodes.Status404NotFound, "Not Found", nf.Message),
            BusinessRuleException br => (StatusCodes.Status422UnprocessableEntity, "Business rule violation", br.Message),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "Внутренняя ошибка сервера")
        };

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = ctx.Request.Path
        };

        ctx.Response.StatusCode = status;
        await ctx.Response.WriteAsJsonAsync(problem, ct);
        return true;
    }
}