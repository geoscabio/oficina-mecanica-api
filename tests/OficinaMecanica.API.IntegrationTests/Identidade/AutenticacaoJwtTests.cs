using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using OficinaMecanica.API.IntegrationTests.Fixtures;

namespace OficinaMecanica.API.IntegrationTests.Identidade;

public sealed class AutenticacaoJwtTests : IClassFixture<OficinaMecanicaApiFixture>
{
    private readonly HttpClient _client;

    public AutenticacaoJwtTests(OficinaMecanicaApiFixture fixture)
    {
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task Dado_EndpointProtegido_Quando_AcessarSemToken_Entao_DeveRetornarNaoAutorizado()
    {
        // Arrange
        const string endpointProtegido = "/api/v1/administrativo/mecanicos/listar";

        // Act
        var response = await _client.GetAsync(endpointProtegido);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Dado_CredenciaisValidas_Quando_Autenticar_Entao_DeveRetornarJwt()
    {
        // Arrange
        var request = CriarRequestLogin("admin", "admin123");

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identidade/autenticacao/login",
            request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();
        var token = content.GetProperty("token").GetString();

        token.Should().NotBeNullOrWhiteSpace();
        token!.Split('.').Should().HaveCount(3);
    }

    [Fact]
    public async Task Dado_PerfilSemPermissao_Quando_AcessarEndpointRestrito_Entao_DeveRetornarProibido()
    {
        // Arrange
        var token = await AutenticarAsync("cliente", "cliente123");

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/administrativo/mecanicos/listar");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<string> AutenticarAsync(string login, string senha)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/v1/identidade/autenticacao/login",
            CriarRequestLogin(login, senha));

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();

        return content.GetProperty("token").GetString()!;
    }

    private static object CriarRequestLogin(string login, string senha)
    {
        return new
        {
            login,
            senha
        };
    }
}
