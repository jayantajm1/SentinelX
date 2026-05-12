using SentinelX.UserService.Data;

namespace SentinelX.UserService.Services;

public interface IUserService
{
    Task<UserProfileDto?> GetProfileAsync(long userId);
    Task<UserProfileDto> UpdateProfileAsync(long userId, UpdateUserProfileDto dto);
    Task<bool> EnableTwoFactorAsync(long userId);
}

public class UserService : IUserService
{
    private readonly UserDbContext _context;

    public UserService(UserDbContext context)
    {
        _context = context;
    }

    public async Task<UserProfileDto?> GetProfileAsync(long userId)
    {
        var profile = await _context.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
            return null;

        return new UserProfileDto
        {
            UserId = profile.UserId,
            PhoneNumber = profile.PhoneNumber,
            Address = profile.Address,
            City = profile.City,
            Country = profile.Country,
            IsVerified = profile.IsVerified,
            TwoFactorEnabled = profile.TwoFactorEnabled
        };
    }

    public async Task<UserProfileDto> UpdateProfileAsync(long userId, UpdateUserProfileDto dto)
    {
        var profile = await _context.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
        {
            profile = new UserProfile { UserId = userId };
            _context.UserProfiles.Add(profile);
        }

        profile.PhoneNumber = dto.PhoneNumber;
        profile.Address = dto.Address;
        profile.City = dto.City;
        profile.Country = dto.Country;
        profile.PostalCode = dto.PostalCode;
        profile.Bio = dto.Bio;
        profile.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new UserProfileDto
        {
            UserId = profile.UserId,
            PhoneNumber = profile.PhoneNumber,
            Address = profile.Address,
            City = profile.City,
            Country = profile.Country,
            IsVerified = profile.IsVerified,
            TwoFactorEnabled = profile.TwoFactorEnabled
        };
    }

    public async Task<bool> EnableTwoFactorAsync(long userId)
    {
        var profile = await _context.UserProfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null)
            return false;

        profile.TwoFactorEnabled = true;
        await _context.SaveChangesAsync();
        return true;
    }
}
