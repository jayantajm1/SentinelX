using BCrypt.Net;
using SentinelX.AuthService.DTOs;
using SentinelX.AuthService.Entities;
using SentinelX.AuthService.Repositories;

namespace SentinelX.AuthService.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string ipAddress);
    Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request);
    Task LogoutAsync(string token);
    Task<TokenResponseDto> RefreshTokenAsync(string refreshToken);
    Task<UserProfileDto?> GetUserProfileAsync(long userId);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, string ipAddress)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        
        if (user == null)
        {
            _logger.LogWarning($"Login attempt with non-existent email: {request.Email}");
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5)
            {
                user.IsLocked = true;
            }
            await _userRepository.UpdateAsync(user);
            _logger.LogWarning($"Failed login attempt for user: {user.Email}");
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        if (user.IsLocked || !user.IsActive)
        {
            _logger.LogWarning($"Login attempt for locked/inactive user: {user.Email}");
            throw new UnauthorizedAccessException("Account is locked or inactive");
        }

        // Reset failed attempts
        user.FailedLoginAttempts = 0;
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        // Get permissions
        var permissions = await _roleRepository.GetUserPermissionsAsync(user.Id);

        // Generate tokens
        var tokens = _tokenService.GenerateTokens(user, permissions);

        _logger.LogInformation($"User logged in successfully: {user.Email}");

        return new LoginResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Username = user.Username,
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            ExpiresAt = tokens.ExpiresAt
        };
    }

    public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        // Check if user already exists
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var user = new User
        {
            Email = request.Email,
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            IsActive = true
        };

        await _userRepository.CreateAsync(user);

        // Assign default User role
        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = 3 // Assuming 3 is the User role ID from init.sql
        };
        
        await _roleRepository.AssignRoleAsync(userRole);

        var permissions = await _roleRepository.GetUserPermissionsAsync(user.Id);
        var tokens = _tokenService.GenerateTokens(user, permissions);

        _logger.LogInformation($"New user registered: {user.Email}");

        return new LoginResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Username = user.Username,
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
            ExpiresAt = tokens.ExpiresAt
        };
    }

    public async Task LogoutAsync(string token)
    {
        // Add token to blacklist in Redis
        _logger.LogInformation("User logged out");
        await Task.CompletedTask;
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public async Task<UserProfileDto?> GetUserProfileAsync(long userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return null;

        var roles = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>();
        var permissions = await _roleRepository.GetUserPermissionsAsync(userId);

        return new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = roles,
            Permissions = permissions,
            CreatedAt = user.CreatedAt
        };
    }
}
