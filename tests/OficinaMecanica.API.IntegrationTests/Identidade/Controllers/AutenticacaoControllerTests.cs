using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using OficinaMecanica.API.IntegrationTests.Fixtures;
using OficinaMecanica.API.IntegrationTests.Identidade.Builders;

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
    }
}
