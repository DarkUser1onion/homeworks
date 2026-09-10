using System.Diagnostics;
using TaskBoard.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ITaskService, InMemoryTaskService>();

var app = builder.Build();

app.Use(async (context, next) =>
{
    var sw = Stopwatch.StartNew();
    context.Response.OnStarting(() =>
    {
        sw.Stop();
        context.Response.Headers.Append("X-Response-Time-ms", sw.ElapsedMilliseconds.ToString());
        return Task.CompletedTask;
    });
    await next(context);
});

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-App-Name", "TaskBoard");
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("--> {Method} {Path}", context.Request.Method, context.Request.Path);
    await next(context);
    logger.LogInformation("<-- {StatusCode}", context.Response.StatusCode);
});

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/health")
    {
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("healthy");
        return;
    }
    await next(context);
});

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/tasks/api"))
    {
        if (!context.Request.Headers.TryGetValue("X-Api-Key", out var apiKey)
            || apiKey != "secret123")
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: invalid or missing X-Api-Key");
            return;
        }
    }
    await next(context);
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();