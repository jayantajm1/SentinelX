namespace SentinelX.Shared.Models;

public class JwtTokenClaims
{
    public long UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public List<string> Permissions { get; set; } = new();
    public string? DeviceId { get; set; }
    public string? IpAddress { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}

public class TokenPair
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class AuditLogEvent
{
    public long? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
    public string? ResourceType { get; set; }
    public string? ResourceId { get; set; }
    public string? IpAddress { get; set; }
    public Guid CorrelationId { get; set; }
    public string Status { get; set; } = "Success";
    public Dictionary<string, object>? Details { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SecurityAlert
{
    public long? UserId { get; set; }
    public string AlertType { get; set; } = string.Empty;
    public string Severity { get; set; } = "Medium"; // Low, Medium, High, Critical
    public string? IpAddress { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
