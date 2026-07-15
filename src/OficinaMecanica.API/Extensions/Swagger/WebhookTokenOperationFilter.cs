using Microsoft.OpenApi;
using OficinaMecanica.API.Extensions.Security;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OficinaMecanica.API.Extensions.Swagger;

internal sealed class WebhookTokenOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;

        if (!metadata.OfType<WebhookTokenAuthorizeAttribute>().Any())
        {
            return;
        }

        operation.Parameters ??= [];
        operation.Parameters.Add(
            new OpenApiParameter
            {
                Name = WebhookTokenAuthorizationFilter.HeaderName,
                In = ParameterLocation.Header,
                Required = true,
                Description = "Token externo configurado em `Integracoes:Orcamento:WebhookToken`."
            });
    }
}
