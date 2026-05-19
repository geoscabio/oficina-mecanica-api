using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using OficinaMecanica.API.IntegrationTests.Identidade.Builders;

namespace OficinaMecanica.API.IntegrationTests.Fixtures;

[Collection(OficinaMecanicaApiCollection.Nome)]
public abstract class ApiIntegrationTestBase : IAsyncLifetime
{
    protected ApiIntegrationTestBase(OficinaMecanicaApiFixture fixture)
    {
        Fixture = fixture;
        Client = fixture.CreateClient();
    }

    protected OficinaMecanicaApiFixture Fixture { get; }
    protected HttpClient Client { get; }

    public virtual async Task InitializeAsync()
    {
        Client.DefaultRequestHeaders.Authorization = null;

        await Fixture.ResetarBancoAsync();
    }

    public virtual Task DisposeAsync()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        Client.Dispose();

        return Task.CompletedTask;
    }

    protected async Task AutenticarComoAdministradorAsync()
    {
        await DefinirTokenAsync("admin", "admin123");
    }

    protected async Task AutenticarComoClienteAsync()
    {
        await DefinirTokenAsync("cliente", "cliente123");
    }

    protected async Task<JsonElement> GetJsonAsync(string endpoint)
    {
        var response = await Client.GetAsync(endpoint);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        return await LerJsonAsync(response);
    }

    protected async Task<JsonElement> PostJsonAsync(
        string endpoint,
        object? request,
        HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
    {
        var response = request is null
            ? await Client.PostAsync(endpoint, content: null)
            : await Client.PostAsJsonAsync(endpoint, request);

        response.StatusCode.Should().Be(expectedStatusCode);

        return await LerJsonAsync(response);
    }

    protected async Task<JsonElement> PutJsonAsync(
        string endpoint,
        object request,
        HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
    {
        var response = await Client.PutAsJsonAsync(endpoint, request);

        response.StatusCode.Should().Be(expectedStatusCode);

        return await LerJsonAsync(response);
    }

    protected async Task<JsonElement> DeleteJsonAsync(
        string endpoint,
        HttpStatusCode expectedStatusCode = HttpStatusCode.OK)
    {
        var response = await Client.DeleteAsync(endpoint);

        response.StatusCode.Should().Be(expectedStatusCode);

        return await LerJsonAsync(response);
    }

    protected static Guid ObterGuid(JsonElement json, string propertyName)
    {
        return json.GetProperty(propertyName).GetGuid();
    }

    protected static string? ObterString(JsonElement json, string propertyName)
    {
        return json.GetProperty(propertyName).GetString();
    }

    protected static void ListagemDevePossuirItens(JsonElement response, int quantidadeMinima = 1)
    {
        response.GetProperty("totalItens").GetInt32().Should().BeGreaterThanOrEqualTo(quantidadeMinima);
        response.GetProperty("itens").GetArrayLength().Should().BeGreaterThanOrEqualTo(quantidadeMinima);
    }

    private async Task DefinirTokenAsync(string login, string senha)
    {
        var token = await AutenticarAsync(login, senha);

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<string> AutenticarAsync(string login, string senha)
    {
        var response = await PostJsonAsync(
            "/api/v1/identidade/autenticacao/login",
            AutenticacaoRequestBuilder.Novo()
                .ComLogin(login)
                .ComSenha(senha)
                .Build(),
            HttpStatusCode.OK);

        return response.GetProperty("token").GetString()!;
    }

    private static async Task<JsonElement> LerJsonAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        content.Should().NotBeNullOrWhiteSpace();

        using var document = JsonDocument.Parse(content);

        return document.RootElement.Clone();
    }
}
