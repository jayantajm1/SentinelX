using SentinelX.Shared.Messaging.Events;

namespace SentinelX.Auth.Domain.Events;

public class UserRegisteredEvent : DomainEvent
{
    public long UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
}

public class UserLoginEvent : DomainEvent
{
    public long UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime LoginAt { get; set; }
}

public class PasswordChangedEvent : DomainEvent
{
    public long UserId { get; set; }
    public DateTime ChangedAt { get; set; }
}

public class RoleAssignedEvent : DomainEvent
{
    public long UserId { get; set; }
    public int RoleId { get; set; }
    public DateTime AssignedAt { get; set; }
}
