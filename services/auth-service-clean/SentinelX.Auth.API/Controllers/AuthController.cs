using Microsoft.AspNetCore.Mvc;
using SentinelX.Auth.Application.DTOs;
using SentinelX.Auth.Application.Features.Login;
using SentinelX.Auth.Application.Features.Register;
using SentinelX.Auth.Application.Features.RefreshToken;
using SentinelX.Auth.Application.Interfaces;
using SentinelX.Shared.Contracts.DTOs;

namespace SentinelX.Auth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthApplicationService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthApplicationService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var command = new LoginCommand
            {
                Email = request.Email,
                Password = request.Password,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = Request.Headers["User-Agent"].ToString()
            };

            var result = await _authService.LoginAsync(command);
            return Ok(ApiResponse<LoginResponseDto>.CreateSuccess(result, "Login successful"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed");
            return Unauthorized(ApiResponse<LoginResponseDto>.CreateFailure("Login failed", "AUTH_FAILED"));
        }
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<RegisterResponseDto>>> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var command = new RegisterCommand
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var result = await _authService.RegisterAsync(command);
            return CreatedAtAction(nameof(Register), ApiResponse<RegisterResponseDto>.CreateSuccess(result, "Registration successful"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration failed");
            return BadRequest(ApiResponse<RegisterResponseDto>.CreateFailure("Registration failed", "REG_FAILED"));
        }
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ApiResponse<RefreshTokenResponseDto>>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            var command = new RefreshTokenCommand { RefreshToken = request.RefreshToken };
            var result = await _authService.RefreshTokenAsync(command);
            return Ok(ApiResponse<RefreshTokenResponseDto>.CreateSuccess(result, "Token refreshed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token refresh failed");
            return Unauthorized(ApiResponse<RefreshTokenResponseDto>.CreateFailure("Token refresh failed", "TOKEN_REFRESH_FAILED"));
        }
    }
}
