using SentinelX.Auth.Application.DTOs;
using SentinelX.Auth.Domain.Interfaces;

namespace SentinelX.Auth.Application.Features.RefreshToken;

public class RefreshTokenCommand
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshTokenCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RefreshTokenResponseDto> HandleAsync(RefreshTokenCommand command)
    {
        var token = await _unitOfWork.RefreshTokenRepository.GetByTokenAsync(command.RefreshToken);
        if (token == null || token.IsRevoked || token.ExpiryDate < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token");
        }

        var user = await _unitOfWork.UserRepository.GetByIdAsync(token.UserId);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        // Generate new tokens
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = new List<string>();
        
        foreach (var userRole in user.UserRoles)
        {
            var rolePerms = await _unitOfWork.RoleRepository.GetPermissionsAsync(userRole.RoleId);
            permissions.AddRange(rolePerms.Select(p => p.Name).Distinct());
        }

        var accessToken = GenerateAccessToken(user.Id, user.Email, roles, permissions);
        var newRefreshToken = GenerateRefreshToken();

        // Update refresh token
        token.Token = newRefreshToken;
        token.ExpiryDate = DateTime.UtcNow.AddDays(7);
        await _unitOfWork.RefreshTokenRepository.UpdateAsync(token);
        await _unitOfWork.SaveChangesAsync();

        return new RefreshTokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = 1800
        };
    }

    private string GenerateAccessToken(long userId, string email, List<string> roles, List<string> permissions)
    {
        return "mock_access_token";
    }

    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
    }
}

public class UnauthorizedAccessException : Exception
{
    public UnauthorizedAccessException(string message) : base(message) { }
}
