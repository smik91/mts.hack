using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Midiot.BL.Automapper;
using Midiot.BL.Interfaces.Auth;
using Midiot.BL.Interfaces.Email;
using Midiot.BL.Interfaces.User;
using Midiot.BL.Services.Auth;
using Midiot.BL.Services.Email;
using Midiot.BL.Services.User;

namespace Midiot.BL;

public static class BLServiceExtensions
{
    public static IServiceCollection ConfigureBLServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddScoped<IUserAccountService, UserAccountService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserExistenceService, UserExistenceService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddSingleton<IEmailService, EmailService>();
        return services;
    }
}
