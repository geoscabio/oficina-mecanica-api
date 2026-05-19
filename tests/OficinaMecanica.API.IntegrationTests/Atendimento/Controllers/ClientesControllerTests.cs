using System.Net;
using FluentAssertions;
using OficinaMecanica.API.IntegrationTests.Atendimento.Builders;
using OficinaMecanica.API.IntegrationTests.Fixtures;

namespace OficinaMecanica.API.IntegrationTests.Atendimento.Controllers;

[Collection(OficinaMecanicaApiCollection.Nome)]
public sealed class ClientesControllerTests : ApiIntegrationTestBase
{
    public ClientesControllerTests(OficinaMecanicaApiFixture fixture)
        : base(fixture)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await AutenticarComoAdministradorAsync();
    }

    [Fact]
    public async Task Dado_ClienteValido_Quando_ExecutarCrud_Entao_DevePersistirAlterarListarERemover()
    {
        // Arrange
        var cadastro = ClienteRequestBuilder.Novo().BuildCadastro();

        // Act
        var clienteCriado = await PostJsonAsync(
            "/api/v1/atendimento/clientes/cadastrar",
            cadastro,
            HttpStatusCode.Created);
        var clienteId = ObterGuid(clienteCriado, "id");

        var clienteConsultado = await GetJsonAsync($"/api/v1/atendimento/clientes/consultar/{clienteId}");
        var clientePorDocumento = await GetJsonAsync(
            $"/api/v1/atendimento/clientes/consultar-por-documento/{cadastro.Documento}");
        var clientes = await GetJsonAsync("/api/v1/atendimento/clientes/listar");

        var atualizacao = ClienteRequestBuilder.Novo()
            .ComNome("Maria Cliente Atualizada")
            .ComTelefone("(11) 98888-7777")
            .ComEmail("maria.atualizada@email.com")
            .BuildAtualizacao(clienteId);
        var clienteAtualizado = await PutJsonAsync(
            $"/api/v1/atendimento/clientes/{clienteId}/atualizar",
            atualizacao);

        await DeleteJsonAsync($"/api/v1/atendimento/clientes/{clienteId}/remover");
        var consultaAposRemocao = await Client.GetAsync($"/api/v1/atendimento/clientes/consultar/{clienteId}");

        // Assert
        ObterString(clienteConsultado, "nome").Should().Be(cadastro.Nome);
        ObterGuid(clientePorDocumento, "id").Should().Be(clienteId);
        ListagemDevePossuirItens(clientes);
        ObterString(clienteAtualizado, "nome").Should().Be(atualizacao.Nome);
        consultaAposRemocao.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
