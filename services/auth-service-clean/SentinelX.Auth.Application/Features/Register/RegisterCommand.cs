using SentinelX.Auth.Application.DTOs;
using SentinelX.Auth.Domain.Entities;
using SentinelX.Auth.Domain.Interfaces;

namespace SentinelX.Auth.Application.Features.Register;

public class RegisterCommand
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class RegisterCommandHandler
{
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RegisterResponseDto> HandleAsync(RegisterCommand command)
    {
        // Verify user doesn't exist
        var existingUser = await _unitOfWork.UserRepository.GetByEmailAsync(command.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var existingUsername = await _unitOfWork.UserRepository.GetByUsernameAsync(command.Username);
        if (existingUsername != null)
        {
            throw new InvalidOperationException("Username already exists");
        }

        // Create new user
        var user = new User
        {
            Username = command.Username,
            Email = command.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
            FirstName = command.FirstName,
            LastName = command.LastName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.UserRepository.AddAsync(user);

        // Assign default user role
        var userRole = new Role { Name = "User" };
        var existingRole = await _unitOfWork.RoleRepository.GetByNameAsync("User");
        if (existingRole == null)
        {
            await _unitOfWork.RoleRepository.AddAsync(userRole);
            await _unitOfWork.SaveChangesAsync();
            existingRole = userRole;
        }

        user.UserRoles.Add(new UserRole { User = user, Role = existingRole });
        await _unitOfWork.UserRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return new RegisterResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }
}
