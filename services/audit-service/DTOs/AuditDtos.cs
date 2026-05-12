namespace SentinelX.AuditService.DTOs;

public class AuditLogDto
{
    public long? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Service { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class SecurityAlertDto
{
    public long? UserId { get; set; }
    public string AlertType { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
