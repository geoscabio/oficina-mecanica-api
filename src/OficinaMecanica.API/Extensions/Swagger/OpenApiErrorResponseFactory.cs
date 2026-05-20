using Microsoft.OpenApi;
using OficinaMecanica.Application.Common;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OficinaMecanica.API.Extensions.Swagger;

internal static class OpenApiErrorResponseFactory
{
    private const string JsonContentType = "application/json";

    public static OpenApiResponse Create(
        OperationFilterContext context,
        string description)
    {
        return new OpenApiResponse
        {
            Description = description,
            Content = new Dictionary<string, OpenApiMediaType>
            {
                [JsonContentType] = new()
                {
                    Schema = context.SchemaGenerator.GenerateSchema(
                        typeof(ErrorResponse),
                        context.SchemaRepository)
                }
            }
        };
    }
}
