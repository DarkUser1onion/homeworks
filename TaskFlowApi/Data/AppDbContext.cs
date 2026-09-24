using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Entities;

namespace TaskFlowApi.Data;

public class AppDbContext : DbContext
{
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<IdempotencyRecord>()
            .HasIndex(x => x.Key)
            .IsUnique();

        b.Entity<TaskItem>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<TaskItem>()
            .HasOne(t => t.AssignedTo)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssignedToId)
            .OnDelete(DeleteBehavior.SetNull);

        b.Entity<Comment>()
            .HasOne(c => c.TaskItem)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TaskItemId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<Comment>()
            .HasOne(c => c.Author)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        b.Entity<TaskItem>().HasIndex(t => t.Status);

        // Seed data
        b.Entity<AppUser>().HasData(new AppUser
        {
            Id = 1,
            Username = "admin",
            Email = "admin@taskflow.local",
            PasswordHash = "seed"
        });

        b.Entity<Project>().HasData(new Project
        {
            Id = 1,
            Name = "Демо-проект",
            Description = "Сид-проект",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        b.Entity<TaskItem>().HasData(new TaskItem
        {
            Id = 1,
            ProjectId = 1,
            Title = "Первая задача",
            Description = "Сид-задача",
            Status = TaskItemStatus.ToDo,
            Priority = TaskPriority.Medium,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}