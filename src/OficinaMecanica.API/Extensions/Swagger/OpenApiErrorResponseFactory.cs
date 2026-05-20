using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using OficinaMecanica.API.Responses;
using OficinaMecanica.Application.Common;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OficinaMecanica.API.Extensions.Swagger;

internal static class OpenApiErrorResponseFactory
{
    public static OpenApiResponse Create(
        OperationFilterContext context,
        string description,
        TipoErro tipoErro)
    {
        var example = new JsonObject
        {
            ["mensagem"] = description,
            ["tipo"] = tipoErro.ToString()
        };

        if (tipoErro == TipoErro.Validacao)
        {
            example["erros"] = new JsonArray(description);
        }

        return new OpenApiResponse
        {
            Description = description,
            Content = new Dictionary<string, OpenApiMediaType>
            {
                [ApiResponseContentTypes.Json] = new()
                {
                    Schema = context.SchemaGenerator.GenerateSchema(
                        typeof(ErrorResponse),
                        context.SchemaRepository),
                    Example = example
                }
            }
        };
    }
}
