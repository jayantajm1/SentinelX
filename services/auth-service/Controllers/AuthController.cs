using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SentinelX.AuthService.DTOs;
using SentinelX.AuthService.Services;
using SentinelX.Shared.DTOs;

namespace SentinelX.AuthService.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var response = await _authService.LoginAsync(request, ipAddress);
            return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning($"Unauthorized login attempt: {ex.Message}");
            return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Login error: {ex.Message}");
            return StatusCode(500, ApiResponse<LoginResponseDto>.ErrorResponse("An error occurred during login"));
        }
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(response, "Registration successful"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning($"Registration error: {ex.Message}");
            return BadRequest(ApiResponse<LoginResponseDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Registration error: {ex.Message}");
            return StatusCode(500, ApiResponse<LoginResponseDto>.ErrorResponse("An error occurred during registration"));
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> Logout()
    {
        try
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            await _authService.LogoutAsync(token);
            return Ok(ApiResponse<object>.SuccessResponse(new { }, "Logout successful"));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Logout error: {ex.Message}");
            return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred during logout"));
        }
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<TokenResponseDto>>> Refresh([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(request.RefreshToken);
            return Ok(ApiResponse<TokenResponseDto>.SuccessResponse(response, "Token refreshed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Token refresh error: {ex.Message}");
            return StatusCode(500, ApiResponse<TokenResponseDto>.ErrorResponse("An error occurred during token refresh"));
        }
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserProfileDto>>> GetProfile()
    {
        try
        {
            var userId = long.Parse(User.FindFirst("sub")?.Value ?? "0");
            var profile = await _authService.GetUserProfileAsync(userId);
            
            if (profile == null)
                return NotFound(ApiResponse<UserProfileDto>.ErrorResponse("User profile not found"));

            return Ok(ApiResponse<UserProfileDto>.SuccessResponse(profile, "Profile retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Get profile error: {ex.Message}");
            return StatusCode(500, ApiResponse<UserProfileDto>.ErrorResponse("An error occurred while retrieving profile"));
        }
    }
}
