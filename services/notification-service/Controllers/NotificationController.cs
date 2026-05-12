using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SentinelX.NotificationService.Data;
using SentinelX.Shared.DTOs;
using Microsoft.EntityFrameworkCore;

namespace SentinelX.NotificationService.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly NotificationDbContext _context;
    private readonly ILogger<NotificationController> _logger;

    public NotificationController(NotificationDbContext context, ILogger<NotificationController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<ApiResponse<List<Notification>>>> GetUserNotifications(long userId)
    {
        try
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Ok(ApiResponse<List<Notification>>.SuccessResponse(notifications));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving notifications: {ex.Message}");
            return StatusCode(500, ApiResponse<List<Notification>>.ErrorResponse("Error retrieving notifications"));
        }
    }

    [HttpPost("{notificationId}/read")]
    public async Task<ActionResult<ApiResponse<object>>> MarkAsRead(long notificationId)
    {
        try
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Notification not found"));

            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Notification marked as read"));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error marking notification as read: {ex.Message}");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error updating notification"));
        }
    }
}
