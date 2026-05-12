using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SentinelX.AuthService.Entities;
using SentinelX.Shared.Models;

namespace SentinelX.AuthService.Services;

public interface ITokenService
{
    TokenPair GenerateTokens(User user, List<string> permissions);
    JwtSecurityToken ValidateToken(string token);
    string GenerateRefreshToken();
    Task<bool> ValidateRefreshTokenAsync(long userId, string refreshToken);
}

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public TokenPair GenerateTokens(User user, List<string> permissions)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]!);
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "30");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("permissions", string.Join(",", permissions))
        };

        var userRoles = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>();
        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key_credential = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(key_credential, SecurityAlgorithms.HmacSha256Signature);
        var expires = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = GenerateRefreshToken();

        return new TokenPair
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expires
        };
    }

    public JwtSecurityToken ValidateToken(string token)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]!);

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return (JwtSecurityToken)validatedToken;
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Token validation failed: {ex.Message}");
            throw;
        }
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public async Task<bool> ValidateRefreshTokenAsync(long userId, string refreshToken)
    {
        // This will be implemented with the repository pattern
        return await Task.FromResult(true);
    }
}
