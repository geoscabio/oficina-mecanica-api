using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OficinaMecanica.API.Extensions.Responses;
using OficinaMecanica.API.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Infrastructure.Identidade.Options;

namespace OficinaMecanica.API.Extensions.Security;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = new JwtOptions
        {
            Issuer = configuration["Jwt:Issuer"] ?? string.Empty,
            Audience = configuration["Jwt:Audience"] ?? string.Empty,
            Secret = configuration["Jwt:Secret"] ?? string.Empty,
            ExpirationMinutes = int.TryParse(configuration["Jwt:ExpirationMinutes"], out var expirationMinutes)
                ? expirationMinutes
                : 60
        };

        jwtOptions.Validar();

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    RoleClaimType = "role",
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                        return context.Response.WriteApiErrorResponseAsJsonAsync(new ErrorResponse(ApiResponseMessages.NaoAutorizado, TipoErro.NaoAutorizado));
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;

                        return context.Response.WriteApiErrorResponseAsJsonAsync(new ErrorResponse(ApiResponseMessages.AcessoProibido, TipoErro.AcessoProibido));
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}
