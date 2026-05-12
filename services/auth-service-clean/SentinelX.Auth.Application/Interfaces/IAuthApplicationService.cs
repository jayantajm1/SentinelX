using SentinelX.Auth.Application.Features.Login;
using SentinelX.Auth.Application.Features.Register;
using SentinelX.Auth.Application.Features.RefreshToken;

namespace SentinelX.Auth.Application.Interfaces;

public interface IAuthApplicationService
{
    Task<LoginResponseDto> LoginAsync(LoginCommand command);
    Task<RegisterResponseDto> RegisterAsync(RegisterCommand command);
    Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenCommand command);
}

public class AuthApplicationService : IAuthApplicationService
{
    private readonly LoginCommandHandler _loginHandler;
    private readonly RegisterCommandHandler _registerHandler;
    private readonly RefreshTokenCommandHandler _refreshTokenHandler;

    public AuthApplicationService(
        LoginCommandHandler loginHandler,
        RegisterCommandHandler registerHandler,
        RefreshTokenCommandHandler refreshTokenHandler)
    {
        _loginHandler = loginHandler;
        _registerHandler = registerHandler;
        _refreshTokenHandler = refreshTokenHandler;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginCommand command)
    {
        return await _loginHandler.HandleAsync(command);
    }

    public async Task<RegisterResponseDto> RegisterAsync(RegisterCommand command)
    {
        return await _registerHandler.HandleAsync(command);
    }

    public async Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenCommand command)
    {
        return await _refreshTokenHandler.HandleAsync(command);
    }
}

// Using types from Features
using SentinelX.Auth.Application.DTOs;
