using System.Net;
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
        RespostaDeErroDevePossuirExemplo(responses, "403", ApiResponseMessages.AcessoProibido, TipoErro.AcessoProibido);
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
    public async Task Dado_EndpointWebhookOrcamento_Quando_GerarSwagger_Entao_DeveDocumentarTokenExternoSemBearer()
    {
        // Arrange
        const string endpoint = "/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/orcamento/notificacoes";

        // Act
        var swagger = await GetJsonAsync("/swagger/v1/swagger.json");

        // Assert
        var operation = swagger
            .GetProperty("paths")
            .GetProperty(endpoint)
            .GetProperty("post");

        operation.TryGetProperty("security", out _).Should().BeFalse();

        operation
            .GetProperty("parameters")
            .EnumerateArray()
            .Should()
            .Contain(parameter =>
                parameter.GetProperty("name").GetString() == "X-Webhook-Token"
                && parameter.GetProperty("in").GetString() == "header"
                && parameter.GetProperty("required").GetBoolean());
    }

    [RequiresDockerFact]
    public async Task Dado_EndpointConsultaStatus_Quando_GerarSwagger_Entao_NaoDeveExigirBearer()
    {
        // Arrange
        const string endpoint = "/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/consultar-status";

        // Act
        var swagger = await GetJsonAsync("/swagger/v1/swagger.json");

        // Assert
        var operation = swagger
            .GetProperty("paths")
            .GetProperty(endpoint)
            .GetProperty("get");

        operation.TryGetProperty("security", out _).Should().BeFalse();
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

    [RequiresDockerFact]
    public async Task Dado_RequisicaoHttp_Quando_ProcessarMiddlewares_Entao_DeveAplicarHeadersDeSeguranca()
    {
        // Act
        var response = await Client.GetAsync("/swagger/index.html");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        HeaderDevePossuirValor(response, "Content-Security-Policy", "default-src 'self'");
        HeaderDevePossuirValor(response, "Cross-Origin-Embedder-Policy", "require-corp");
        HeaderDevePossuirValor(response, "Cross-Origin-Opener-Policy", "same-origin");
        HeaderDevePossuirValor(response, "Cross-Origin-Resource-Policy", "same-origin");
        HeaderDevePossuirValor(response, "Permissions-Policy", "camera=(), geolocation=(), microphone=()");
        HeaderDevePossuirValor(response, "X-Content-Type-Options", "nosniff");
        HeaderDevePossuirValor(response, "X-Frame-Options", "DENY");
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

    private static void RespostaDeErroDevePossuirExemplo(JsonElement responses, string statusCode, string mensagem, TipoErro tipoErro)
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

    private static void HeaderDevePossuirValor(HttpResponseMessage response, string headerName, string expectedValue)
    {
        response.Headers.TryGetValues(headerName, out var values).Should().BeTrue();
        values.Should().ContainSingle().Which.Should().Contain(expectedValue);
    }
}

