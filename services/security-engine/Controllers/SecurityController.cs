using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SentinelX.SecurityEngine.Services;
using SentinelX.Shared.DTOs;

namespace SentinelX.SecurityEngine.Controllers;

[ApiController]
[Route("api/security")]
public class SecurityController : ControllerBase
{
    private readonly IRateLimitService _rateLimitService;
    private readonly ISecurityService _securityService;
    private readonly ILogger<SecurityController> _logger;

    public SecurityController(
        IRateLimitService rateLimitService,
        ISecurityService securityService,
        ILogger<SecurityController> logger)
    {
        _rateLimitService = rateLimitService;
        _securityService = securityService;
        _logger = logger;
    }

    [HttpGet("rate-limit/{userId}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> CheckRateLimit(string userId)
    {
        try
        {
            var isAllowed = await _rateLimitService.IsRequestAllowedAsync(userId, 100, 60);
            var remaining = await _rateLimitService.GetRemainingRequestsAsync(userId);

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                allowed = isAllowed,
                remaining = remaining
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error checking rate limit: {ex.Message}");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error checking rate limit"));
        }
    }

    [HttpPost("check-suspicious/{userId}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> CheckSuspiciousActivity(string userId, [FromQuery] string ipAddress)
    {
        try
        {
            var isSuspicious = await _securityService.DetectSuspiciousActivityAsync(userId, ipAddress);
            
            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                suspicious = isSuspicious
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error checking suspicious activity: {ex.Message}");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error checking suspicious activity"));
        }
    }

    [HttpPost("block-ip/{ipAddress}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> BlockIp(string ipAddress)
    {
        try
        {
            await _securityService.BlockIpAsync(ipAddress);
            return Ok(ApiResponse<object>.SuccessResponse(new { }, "IP blocked successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error blocking IP: {ex.Message}");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error blocking IP"));
        }
    }
}
