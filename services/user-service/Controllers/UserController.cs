using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SentinelX.UserService.Services;
using SentinelX.Shared.DTOs;

namespace SentinelX.UserService.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetProfile(long userId)
    {
        try
        {
            var profile = await _userService.GetProfileAsync(userId);
            if (profile == null)
                return NotFound(ApiResponse<UserProfileDto>.ErrorResponse("User profile not found"));

            return Ok(ApiResponse<UserProfileDto>.SuccessResponse(profile));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving profile: {ex.Message}");
            return StatusCode(500, ApiResponse<UserProfileDto>.ErrorResponse("Error retrieving profile"));
        }
    }

    [HttpPut("{userId}")]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> UpdateProfile(long userId, [FromBody] UpdateUserProfileDto dto)
    {
        try
        {
            var profile = await _userService.UpdateProfileAsync(userId, dto);
            return Ok(ApiResponse<UserProfileDto>.SuccessResponse(profile, "Profile updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating profile: {ex.Message}");
            return StatusCode(500, ApiResponse<UserProfileDto>.ErrorResponse("Error updating profile"));
        }
    }

    [HttpPost("{userId}/two-factor")]
    public async Task<ActionResult<ApiResponse<object>>> EnableTwoFactor(long userId)
    {
        try
        {
            var result = await _userService.EnableTwoFactorAsync(userId);
            if (!result)
                return BadRequest(ApiResponse<object>.ErrorResponse("Could not enable two-factor"));

            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Two-factor enabled successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error enabling two-factor: {ex.Message}");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("Error enabling two-factor"));
        }
    }
}
