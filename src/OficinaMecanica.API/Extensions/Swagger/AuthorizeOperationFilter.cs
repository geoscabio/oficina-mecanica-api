using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using OficinaMecanica.API.Extensions.Security;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OficinaMecanica.API.Extensions.Swagger;

internal sealed class AuthorizeOperationFilter : IOperationFilter
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
            new OpenApiResponse { Description = JwtErrorMessages.NaoAutorizado });
        operation.Responses.TryAdd(
            StatusCodes.Status403Forbidden.ToString(),
            new OpenApiResponse { Description = JwtErrorMessages.AcessoProibido });

        operation.Security ??= [];
        operation.Security.Add(
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(JwtSwaggerExtensions.BearerScheme, context.Document, null)] = []
            });
    }
}
