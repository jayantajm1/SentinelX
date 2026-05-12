using SentinelX.Auth.Domain.Entities;

namespace SentinelX.Auth.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(long id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<List<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(long id);
}

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(int id);
    Task<Role?> GetByNameAsync(string name);
    Task<List<Permission>> GetPermissionsAsync(int roleId);
    Task AddAsync(Role role);
    Task UpdateAsync(Role role);
}

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<List<RefreshToken>> GetByUserIdAsync(long userId);
    Task AddAsync(RefreshToken token);
    Task UpdateAsync(RefreshToken token);
}

public interface ILoginHistoryRepository
{
    Task AddAsync(LoginHistory history);
    Task<List<LoginHistory>> GetByUserIdAsync(long userId, int take = 10);
}

public interface IUnitOfWork : IDisposable
{
    IUserRepository UserRepository { get; }
    IRoleRepository RoleRepository { get; }
    IRefreshTokenRepository RefreshTokenRepository { get; }
    ILoginHistoryRepository LoginHistoryRepository { get; }
    Task<int> SaveChangesAsync();
}
