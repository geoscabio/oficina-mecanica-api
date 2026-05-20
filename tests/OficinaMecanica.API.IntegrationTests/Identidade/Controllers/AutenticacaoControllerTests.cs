using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using OficinaMecanica.API.IntegrationTests.Fixtures;
using OficinaMecanica.API.IntegrationTests.Identidade.Builders;
using OficinaMecanica.API.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.Identidade.ValidationMessages;

namespace OficinaMecanica.API.IntegrationTests.Identidade.Controllers;

[Collection(OficinaMecanicaApiCollection.Nome)]
public sealed class AutenticacaoControllerTests : ApiIntegrationTestBase
{
    public AutenticacaoControllerTests(OficinaMecanicaApiFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task Dado_CredenciaisValidas_Quando_Autenticar_Entao_DeveRetornarJwt()
    {
        // Arrange
        var request = AutenticacaoRequestBuilder.Novo().Build();

        // Act
        var response = await PostJsonAsync(
            "/api/v1/identidade/autenticacao/login",
            request,
            HttpStatusCode.OK);

        // Assert
        var token = ObterString(response, "token");

        token.Should().NotBeNullOrWhiteSpace();
        token!.Split('.').Should().HaveCount(3);
    }

    [Fact]
    public async Task Dado_MesmoUsuarioDemo_Quando_AutenticarDuasVezes_Entao_DeveRetornarMesmoUsuarioId()
    {
        // Arrange
        var request = AutenticacaoRequestBuilder.Novo().Build();

        // Act
        var primeiraAutenticacao = await PostJsonAsync(
            "/api/v1/identidade/autenticacao/login",
            request,
            HttpStatusCode.OK);

        var segundaAutenticacao = await PostJsonAsync(
            "/api/v1/identidade/autenticacao/login",
            request,
            HttpStatusCode.OK);

        // Assert
        ObterString(primeiraAutenticacao, "usuarioId")
            .Should()
            .Be(ObterString(segundaAutenticacao, "usuarioId"));
    }

    [Fact]
    public async Task Dado_CredenciaisInvalidas_Quando_Autenticar_Entao_DeveRetornarNaoAutorizado()
    {
        // Arrange
        var request = AutenticacaoRequestBuilder.Novo()
            .ComSenha("senha-invalida")
            .Build();

        // Act
        var response = await Client.PostAsJsonAsync("/api/v1/identidade/autenticacao/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var erro = await response.Content.ReadFromJsonAsync<JsonElement>();

        ErroDeveSerNaoAutorizado(erro, IdentidadeValidationMessages.CredenciaisInvalidas);
    }

    [Fact]
    public async Task Dado_EndpointProtegido_Quando_AcessarSemToken_Entao_DeveRetornarNaoAutorizado()
    {
        // Arrange
        const string endpoint = "/api/v1/administrativo/mecanicos/listar";

        // Act
        var response = await Client.GetAsync(endpoint);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var erro = await response.Content.ReadFromJsonAsync<JsonElement>();

        ErroDeveSer(erro, ApiResponseMessages.NaoAutorizado, TipoErro.NaoAutorizado);
    }

    [Fact]
    public async Task Dado_PerfilSemPermissao_Quando_AcessarEndpointRestrito_Entao_DeveRetornarProibido()
    {
        // Arrange
        await AutenticarComoClienteAsync();

        // Act
        var response = await Client.GetAsync("/api/v1/administrativo/mecanicos/listar");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var erro = await response.Content.ReadFromJsonAsync<JsonElement>();

        ErroDeveSer(erro, ApiResponseMessages.AcessoProibido, TipoErro.AcessoProibido);
    }

    private static void ErroDeveSerNaoAutorizado(JsonElement erro, string mensagem)
    {
        ErroDeveSer(erro, mensagem, TipoErro.NaoAutorizado);
    }

    private static void ErroDeveSer(JsonElement erro, string mensagem, TipoErro tipoErro)
    {
        erro.GetProperty("tipo").GetString().Should().Be(tipoErro.ToString());
        erro.GetProperty("mensagem").GetString().Should().Be(mensagem);
    }
}
