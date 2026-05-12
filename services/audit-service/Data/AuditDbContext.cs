using Microsoft.EntityFrameworkCore;

namespace SentinelX.AuditService.Data;

public class AuditLog
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
    public string? ResourceType { get; set; }
    public string? ResourceId { get; set; }
    public string? IpAddress { get; set; }
    public Guid CorrelationId { get; set; }
    public string Status { get; set; } = "Success";
    public string? Details { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SecurityAlert
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public string AlertType { get; set; } = string.Empty;
    public string Severity { get; set; } = "Medium";
    public string? IpAddress { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool Resolved { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AuditDbContext : DbContext
{
    public AuditDbContext(DbContextOptions<AuditDbContext> options) : base(options)
    {
    }

    public DbSet<AuditLog> AuditLogs { get; set; } = null!;
    public DbSet<SecurityAlert> SecurityAlerts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.CorrelationId);
        });

        modelBuilder.Entity<SecurityAlert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Severity);
        });
    }
}
