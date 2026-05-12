using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SentinelX.AuditService.Data;
using SentinelX.AuditService.DTOs;
using SentinelX.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace SentinelX.AuditService.Controllers;

[ApiController]
[Route("api/audit")]
[Authorize]
public class AuditController : ControllerBase
{
    private readonly AuditDbContext _context;
    private readonly ILogger<AuditController> _logger;

    public AuditController(AuditDbContext context, ILogger<AuditController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("logs")]
    public async Task<ActionResult<ApiResponse<List<AuditLogDto>>>> GetAuditLogs(int page = 1, int pageSize = 50)
    {
        try
        {
            var logs = await _context.AuditLogs
                .OrderByDescending(l => l.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new AuditLogDto
                {
                    UserId = l.UserId,
                    Action = l.Action,
                    Service = l.Service,
                    IpAddress = l.IpAddress,
                    CorrelationId = l.CorrelationId,
                    CreatedAt = l.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<List<AuditLogDto>>.SuccessResponse(logs));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving audit logs: {ex.Message}");
            return StatusCode(500, ApiResponse<List<AuditLogDto>>.ErrorResponse("Error retrieving audit logs"));
        }
    }

    [HttpGet("alerts")]
    public async Task<ActionResult<ApiResponse<List<SecurityAlertDto>>>> GetSecurityAlerts(int page = 1, int pageSize = 50)
    {
        try
        {
            var alerts = await _context.SecurityAlerts
                .Where(a => !a.Resolved)
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new SecurityAlertDto
                {
                    UserId = a.UserId,
                    AlertType = a.AlertType,
                    Severity = a.Severity,
                    Description = a.Description,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponse<List<SecurityAlertDto>>.SuccessResponse(alerts));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving alerts: {ex.Message}");
            return StatusCode(500, ApiResponse<List<SecurityAlertDto>>.ErrorResponse("Error retrieving alerts"));
        }
    }
}
