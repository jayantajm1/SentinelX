namespace SentinelX.Shared.Constants;

public static class JwtClaimTypes
{
    public const string UserId = "sub";
    public const string Email = "email";
    public const string Username = "username";
    public const string Role = "role";
    public const string Permissions = "permissions";
    public const string DeviceId = "device_id";
    public const string IpAddress = "ip_address";
    public const string TenantId = "tenant_id";
}

public static class RabbitMqQueues
{
    public const string AuditLogQueue = "audit-log-queue";
    public const string EmailNotificationQueue = "email-notification-queue";
    public const string SuspiciousActivityQueue = "suspicious-activity-queue";
    public const string UserCreatedQueue = "user-created-queue";
    public const string LoginEventsQueue = "login-events-queue";
    public const string SecurityAlertQueue = "security-alert-queue";
    
    public const string AuditLogExchange = "audit-exchange";
    public const string NotificationExchange = "notification-exchange";
    public const string SecurityExchange = "security-exchange";
}

public static class CacheKeys
{
    public const string TokenBlacklist = "token-blacklist:{0}";
    public const string RefreshToken = "refresh-token:{0}";
    public const string UserPermissions = "user-permissions:{0}";
    public const string RateLimitKey = "rate-limit:{0}:{1}";
    public const string SessionMetadata = "session:{0}";
}

public static class HttpHeaders
{
    public const string CorrelationId = "X-Correlation-ID";
    public const string Authorization = "Authorization";
    public const string ContentEncryption = "X-Content-Encryption";
    public const string RequestSignature = "X-Request-Signature";
    public const string DeviceId = "X-Device-ID";
}
