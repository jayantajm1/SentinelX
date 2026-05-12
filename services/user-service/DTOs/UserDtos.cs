namespace SentinelX.UserService.DTOs;

public class UserProfileDto
{
    public long UserId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public bool IsVerified { get; set; }
    public bool TwoFactorEnabled { get; set; }
}

public class UpdateUserProfileDto
{
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? Bio { get; set; }
}
