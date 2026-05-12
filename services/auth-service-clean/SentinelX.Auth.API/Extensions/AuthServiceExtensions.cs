using Microsoft.EntityFrameworkCore;
using SentinelX.Auth.Application.Interfaces;
using SentinelX.Auth.Application.Features.Login;
using SentinelX.Auth.Application.Features.Register;
using SentinelX.Auth.Application.Features.RefreshToken;
using SentinelX.Auth.Domain.Interfaces;
using SentinelX.Auth.Infrastructure.Data;
using SentinelX.Auth.Infrastructure.Repositories;

namespace SentinelX.Auth.API.Extensions;

public static class AuthServiceExtensions
{
    public static void AddAuthServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("AuthDb");
        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Repositories & UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Command Handlers
        services.AddScoped<LoginCommandHandler>();
        services.AddScoped<RegisterCommandHandler>();
        services.AddScoped<RefreshTokenCommandHandler>();

        // Application Services
        services.AddScoped<IAuthApplicationService, AuthApplicationService>();
    }
}
