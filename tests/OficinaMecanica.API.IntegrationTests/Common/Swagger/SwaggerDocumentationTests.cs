using System.Text.Json;
using FluentAssertions;
using OficinaMecanica.API.IntegrationTests.Fixtures;

namespace OficinaMecanica.API.IntegrationTests.Common.Swagger;

[Collection(OficinaMecanicaApiCollection.Nome)]
public sealed class SwaggerDocumentationTests : ApiIntegrationTestBase
{
    public SwaggerDocumentationTests(OficinaMecanicaApiFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Dado_EndpointProtegido_Quando_GerarSwagger_Entao_DeveDocumentarRespostasDeErroPadronizadas()
    {
        // Arrange
        const string endpoint = "/api/v1/administrativo/mecanicos/listar";

        // Act
        var swagger = await GetJsonAsync("/swagger/v1/swagger.json");

        // Assert
        var responses = swagger
            .GetProperty("paths")
            .GetProperty(endpoint)
            .GetProperty("get")
            .GetProperty("responses");

        RespostaDeErroDevePossuirSchema(responses, "400");
        RespostaDeErroDevePossuirSchema(responses, "401");
        RespostaDeErroDevePossuirSchema(responses, "403");
        RespostaDeErroDevePossuirSchema(responses, "404");
        RespostaDeErroDevePossuirSchema(responses, "422");
        RespostaDeErroDevePossuirSchema(responses, "500");
    }

    private static void RespostaDeErroDevePossuirSchema(JsonElement responses, string statusCode)
    {
        var response = responses.GetProperty(statusCode);

        response
            .GetProperty("content")
            .GetProperty("application/json")
            .GetProperty("schema")
            .ValueKind
            .Should()
            .NotBe(JsonValueKind.Undefined);
    }
}
