using Microsoft.OpenApi;
using OficinaMecanica.API.Responses;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OficinaMecanica.API.Extensions.Swagger;

internal static class OpenApiSuccessResponseFactory
{
    public static OpenApiResponse Create(OperationFilterContext context, string description, Type? responseType)
    {
        var response = new OpenApiResponse
        {
            Description = description
        };

        if (responseType is null)
        {
            return response;
        }

        response.Content = new Dictionary<string, OpenApiMediaType>
        {
            [ApiResponseContentTypes.Json] = new()
            {
                Schema = context.SchemaGenerator.GenerateSchema(responseType, context.SchemaRepository)
            }
        };

        return response;
    }

    public static OpenApiResponse CreateNoContent(string description)
    {
        return new OpenApiResponse
        {
            Description = description
        };
    }
}
