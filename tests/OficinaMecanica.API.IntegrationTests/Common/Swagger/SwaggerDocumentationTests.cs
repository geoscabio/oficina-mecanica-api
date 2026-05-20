using System.Text.Json;
using FluentAssertions;
using OficinaMecanica.API.IntegrationTests.Fixtures;
using OficinaMecanica.API.Responses;
using OficinaMecanica.Application.Common;

namespace OficinaMecanica.API.IntegrationTests.Common.Swagger;

[Collection(OficinaMecanicaApiCollection.Nome)]
public sealed class SwaggerDocumentationTests : ApiIntegrationTestBase
{
    public SwaggerDocumentationTests(OficinaMecanicaApiFixture fixture)
        : base(fixture)
    {
    }

    [RequiresDockerFact]
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
        RespostaDeErroValidacaoDevePossuirListaErros(responses);
        RespostaDeErroDevePossuirExemplo(
            responses,
            "403",
            ApiResponseMessages.AcessoProibido,
            TipoErro.AcessoProibido);
    }

    [RequiresDockerFact]
    public async Task Dado_EndpointCadastro_Quando_GerarSwagger_Entao_DeveDocumentarCreatedComSchema()
    {
        // Arrange
        const string endpoint = "/api/v1/atendimento/clientes/cadastrar";

        // Act
        var swagger = await GetJsonAsync("/swagger/v1/swagger.json");

        // Assert
        var responses = swagger
            .GetProperty("paths")
            .GetProperty(endpoint)
            .GetProperty("post")
            .GetProperty("responses");

        var response = responses.GetProperty("201");

        response.GetProperty("description").GetString().Should().Be(ApiResponseMessages.RecursoCriadoComSucesso);
        response
            .GetProperty("content")
            .GetProperty(ApiResponseContentTypes.Json)
            .GetProperty("schema")
            .ValueKind
            .Should()
            .NotBe(JsonValueKind.Undefined);
    }

    [RequiresDockerFact]
    public async Task Dado_EndpointRemocao_Quando_GerarSwagger_Entao_DeveDocumentarNoContentSemBody()
    {
        // Arrange
        const string endpoint = "/api/v1/atendimento/clientes/{clienteId}/remover";

        // Act
        var swagger = await GetJsonAsync("/swagger/v1/swagger.json");

        // Assert
        var responses = swagger
            .GetProperty("paths")
            .GetProperty(endpoint)
            .GetProperty("delete")
            .GetProperty("responses");

        responses.TryGetProperty("200", out _).Should().BeFalse();

        var response = responses.GetProperty("204");

        response.GetProperty("description").GetString().Should().Be(ApiResponseMessages.RecursoRemovidoComSucesso);
        response.TryGetProperty("content", out _).Should().BeFalse();
    }

    private static void RespostaDeErroDevePossuirSchema(JsonElement responses, string statusCode)
    {
        var response = responses.GetProperty(statusCode);

        response
            .GetProperty("content")
            .GetProperty(ApiResponseContentTypes.Json)
            .GetProperty("schema")
            .ValueKind
            .Should()
            .NotBe(JsonValueKind.Undefined);
    }

    private static void RespostaDeErroDevePossuirExemplo(
        JsonElement responses,
        string statusCode,
        string mensagem,
        TipoErro tipoErro)
    {
        var example = responses
            .GetProperty(statusCode)
            .GetProperty("content")
            .GetProperty(ApiResponseContentTypes.Json)
            .GetProperty("example");

        example.GetProperty("mensagem").GetString().Should().Be(mensagem);
        example.GetProperty("tipo").GetString().Should().Be(tipoErro.ToString());
    }

    private static void RespostaDeErroValidacaoDevePossuirListaErros(JsonElement responses)
    {
        var example = responses
            .GetProperty("400")
            .GetProperty("content")
            .GetProperty(ApiResponseContentTypes.Json)
            .GetProperty("example");

        example.GetProperty("erros").EnumerateArray()
            .Should()
            .NotBeEmpty();
    }
}

