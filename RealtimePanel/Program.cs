using RealtimePanel.Hubs;
using RealtimePanel.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddSingleton<ConnectionTracker>();

builder.Services.AddScoped<ActivityHub>();

builder.Services.AddCors(o =>
{
    o.AddPolicy("frontend", p => p
        .WithOrigins("http://localhost:3000", "http://localhost:5175")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("frontend");

app.MapHub<ActivityHub>("/hubs/activity");

app.MapPost("/api/system/message", async (ConnectionTracker tracker, SystemMessageDto dto) =>
{
    if (string.IsNullOrWhiteSpace(dto.Text))
        return Results.BadRequest(new { error = "Text is required" });

    await tracker.SendSystemMessageAsync(dto.Text);
    return Results.Ok(new { sent = true });
});

app.Run();

public record SystemMessageDto(string Text);