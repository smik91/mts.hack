using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Midiot.Common.Serilog;
using Midiot.Common.Options;
using Midiot.Data;
using Midiot.BL;

namespace Midiot.API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
        });

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog();
        });

        
        services.Configure<JwtOptions>(configuration.GetSection(key: nameof(JwtOptions)));
        services.Configure<SendGridOptions>(configuration.GetSection(key: nameof(SendGridOptions)));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtOptions:SecretKey"])),
                    ValidIssuer = configuration["JwtOptions:Issuer"]
                };
            });

        services.AddAuthorization();

        services.AddSingleton<ILoggerFactory>(_ => SerilogFactory.InitLogging());
        services.AddHttpContextAccessor();
        services.ConfigureBLServices(configuration);
        services.ConfigureDataServices(configuration);
        return services;
    }
}
