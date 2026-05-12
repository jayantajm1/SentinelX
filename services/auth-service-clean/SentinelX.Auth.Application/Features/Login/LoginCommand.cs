using SentinelX.Auth.Application.DTOs;
using SentinelX.Auth.Domain.Interfaces;

namespace SentinelX.Auth.Application.Features.Login;

public class LoginCommand
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}

public class LoginCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<LoginResponseDto> HandleAsync(LoginCommand command)
    {
        var user = await _unitOfWork.UserRepository.GetByEmailAsync(command.Email);
        if (user == null || !VerifyPassword(command.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        var roles = new List<string>();
        var permissions = new List<string>();

        foreach (var userRole in user.UserRoles)
        {
            roles.Add(userRole.Role.Name);
            var rolePermissions = await _unitOfWork.RoleRepository.GetPermissionsAsync(userRole.RoleId);
            permissions.AddRange(rolePermissions.Select(p => p.Name).Distinct());
        }

        // Generate tokens (simplified - would use ITokenService in real implementation)
        var accessToken = GenerateAccessToken(user.Id, user.Email, roles, permissions);
        var refreshToken = GenerateRefreshToken();

        user.LastLoginAt = DateTime.UtcNow;
        await _unitOfWork.UserRepository.UpdateAsync(user);

        var loginHistory = new Domain.Entities.LoginHistory
        {
            UserId = user.Id,
            IpAddress = command.IpAddress ?? "Unknown",
            UserAgent = command.UserAgent ?? "Unknown",
            IsSuccessful = true
        };
        await _unitOfWork.LoginHistoryRepository.AddAsync(loginHistory);
        await _unitOfWork.SaveChangesAsync();

        return new LoginResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 1800
        };
    }

    private bool VerifyPassword(string password, string hash)
    {
        // Use BCrypt or similar in production
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    private string GenerateAccessToken(long userId, string email, List<string> roles, List<string> permissions)
    {
        // Implement JWT generation
        return "mock_access_token";
    }

    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
    }
}

// Exception types for clarity
public class UnauthorizedAccessException : Exception
{
    public UnauthorizedAccessException(string message) : base(message) { }
}
