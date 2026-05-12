using Microsoft.EntityFrameworkCore;

namespace SentinelX.UserService.Data;

public class UserProfile
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public bool IsVerified { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class UserActivity
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string? ActivityType { get; set; }
    public string? Description { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    public DbSet<UserProfile> UserProfiles { get; set; } = null!;
    public DbSet<UserActivity> UserActivities { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
        });

        modelBuilder.Entity<UserActivity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}
