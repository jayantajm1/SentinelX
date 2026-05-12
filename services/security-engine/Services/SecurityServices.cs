using StackExchange.Redis;

namespace SentinelX.SecurityEngine.Services;

public interface IRateLimitService
{
    Task<bool> IsRequestAllowedAsync(string key, int limit, int windowSeconds);
    Task<int> GetRemainingRequestsAsync(string key);
}

public class RateLimitService : IRateLimitService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RateLimitService> _logger;

    public RateLimitService(IConnectionMultiplexer redis, ILogger<RateLimitService> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<bool> IsRequestAllowedAsync(string key, int limit, int windowSeconds)
    {
        var db = _redis.GetDatabase();
        var redisKey = $"rate-limit:{key}";

        var current = await db.StringIncrementAsync(redisKey);
        if (current == 1)
        {
            await db.KeyExpireAsync(redisKey, TimeSpan.FromSeconds(windowSeconds));
        }

        if (current <= limit)
        {
            return true;
        }

        _logger.LogWarning($"Rate limit exceeded for key: {key}");
        return false;
    }

    public async Task<int> GetRemainingRequestsAsync(string key)
    {
        var db = _redis.GetDatabase();
        var redisKey = $"rate-limit:{key}";
        var current = await db.StringGetAsync(redisKey);

        if (current.IsNullOrEmpty)
            return 100; // Default limit

        var limit = 100;
        return Math.Max(0, limit - (int)current);
    }
}

public interface ISecurityService
{
    Task<bool> DetectSuspiciousActivityAsync(string userId, string ipAddress);
    Task<bool> CheckBruteForceAsync(string email);
    Task BlockIpAsync(string ipAddress);
}

public class SecurityService : ISecurityService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<SecurityService> _logger;

    public SecurityService(IConnectionMultiplexer redis, ILogger<SecurityService> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<bool> DetectSuspiciousActivityAsync(string userId, string ipAddress)
    {
        var db = _redis.GetDatabase();
        var key = $"user-activity:{userId}";
        
        // Implement anomaly detection logic
        _logger.LogInformation($"Checking suspicious activity for user: {userId} from IP: {ipAddress}");
        
        await db.StringIncrementAsync(key);
        await db.KeyExpireAsync(key, TimeSpan.FromHours(1));

        return false; // No suspicious activity detected
    }

    public async Task<bool> CheckBruteForceAsync(string email)
    {
        var db = _redis.GetDatabase();
        var key = $"brute-force:{email}";
        
        var attempts = await db.StringGetAsync(key);
        if (attempts.IsNullOrEmpty)
            return false;

        return (int)attempts >= 5;
    }

    public async Task BlockIpAsync(string ipAddress)
    {
        var db = _redis.GetDatabase();
        var key = $"blocked-ip:{ipAddress}";
        
        await db.StringSetAsync(key, "1", TimeSpan.FromHours(24));
        _logger.LogWarning($"IP blocked: {ipAddress}");
    }
}
