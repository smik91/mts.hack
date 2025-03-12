using Microsoft.OpenApi.Models;

namespace Midiot.API.Extensions;

public static class SwaggerServiceExtensions
{
    public static void AddSwaggerWithJwtAuth(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            options.OperationFilter<AuthorizeOperationFilter>();
        });
    }
}
