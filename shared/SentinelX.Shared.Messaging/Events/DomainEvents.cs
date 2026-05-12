namespace SentinelX.Shared.Messaging.Events;

public abstract class DomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public string EventType => GetType().Name;
}

public class UserCreatedEvent : DomainEvent
{
    public long UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class UserAuthenticatedEvent : DomainEvent
{
    public long UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime AuthenticatedAt { get; set; }
}

public class AuditLogCreatedEvent : DomainEvent
{
    public long UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public long EntityId { get; set; }
    public DateTime Timestamp { get; set; }
}
