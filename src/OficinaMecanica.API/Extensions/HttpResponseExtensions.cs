using System.Text.Json;
using System.Text.Json.Serialization;
using OficinaMecanica.Application.Common;

namespace OficinaMecanica.API.Extensions;

public static class HttpResponseExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public static Task WriteErrorResponseAsJsonAsync(
        this HttpResponse response,
        ErrorResponse error)
    {
        response.ContentType = "application/json";

        return response.WriteAsJsonAsync(error, JsonOptions);
    }
}
