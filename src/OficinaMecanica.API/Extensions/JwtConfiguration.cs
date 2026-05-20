using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Infrastructure.Identidade.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OficinaMecanica.API.Extensions;

public static class JwtConfiguration
{
    private const string BearerScheme = "Bearer";
    private const string MensagemNaoAutorizado = "Não autorizado.";
    private const string MensagemAcessoProibido = "Acesso proibido para o perfil informado.";

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtOptions = new JwtOptions
        {
            Issuer = configuration["Jwt:Issuer"] ?? string.Empty,
            Audience = configuration["Jwt:Audience"] ?? string.Empty,
            Secret = configuration["Jwt:Secret"] ?? string.Empty,
            ExpirationMinutes = int.TryParse(configuration["Jwt:ExpirationMinutes"], out var expirationMinutes)
                ? expirationMinutes
                : 120
        };

        jwtOptions.Validar();

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(2)
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                        return context.Response.WriteErrorResponseAsJsonAsync(
                            new ErrorResponse(MensagemNaoAutorizado, TipoErro.NaoAutorizado));
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;

                        return context.Response.WriteErrorResponseAsJsonAsync(
                            new ErrorResponse(MensagemAcessoProibido, TipoErro.NaoAutorizado));
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }

    public static void AddJwtSwagger(this SwaggerGenOptions options)
    {
        options.AddSecurityDefinition(
            BearerScheme,
            new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Cole apenas o token JWT gerado no login."
            });

        options.OperationFilter<AuthorizeOperationFilter>();
    }

    private sealed class AuthorizeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;

            if (metadata.OfType<IAllowAnonymous>().Any())
            {
                return;
            }

            if (!metadata.OfType<IAuthorizeData>().Any())
            {
                return;
            }

            operation.Responses ??= [];
            operation.Responses.TryAdd(
                StatusCodes.Status401Unauthorized.ToString(),
                new OpenApiResponse { Description = MensagemNaoAutorizado });
            operation.Responses.TryAdd(
                StatusCodes.Status403Forbidden.ToString(),
                new OpenApiResponse { Description = MensagemAcessoProibido });

            operation.Security ??= [];
            operation.Security.Add(
                new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(BearerScheme, context.Document, null)] = []
                });
        }
    }
}
