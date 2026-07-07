using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OficinaMecanica.API.Extensions.Swagger;

public static class JwtSwaggerExtensions
{
    internal const string BearerScheme = "Bearer";

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
                Description = SwaggerDocumentationMessages.TokenJwt
            });

        options.OperationFilter<ApiSuccessResponsesOperationFilter>();
        options.OperationFilter<ApiErrorResponsesOperationFilter>();
        options.OperationFilter<AuthorizeOperationFilter>();
        options.OperationFilter<WebhookTokenOperationFilter>();
    }
}
