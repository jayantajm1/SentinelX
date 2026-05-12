using Microsoft.EntityFrameworkCore;

namespace SentinelX.NotificationService.Data;

public class Notification
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class EmailQueue
{
    public long Id { get; set; }
    public string RecipientEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public int RetryCount { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
    }

    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<EmailQueue> EmailQueues { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IsRead);
        });

        modelBuilder.Entity<EmailQueue>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Status);
        });
    }
}
