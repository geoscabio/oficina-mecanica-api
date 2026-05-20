using System.Net;
using FluentAssertions;
using OficinaMecanica.API.IntegrationTests.Atendimento.Builders;
using OficinaMecanica.API.IntegrationTests.Fixtures;
using OficinaMecanica.Application.Atendimento.ValidationMessages;
using OficinaMecanica.Application.Common;

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

    [RequiresDockerFact]
    public async Task Dado_ClienteValido_Quando_ExecutarCrud_Entao_DevePersistirAlterarListarERemover()
    {
        // Arrange
        var cadastro = ClienteRequestBuilder.Novo().BuildCadastro();

        // Act
        var clienteCriado = await PostJsonAsync("/api/v1/atendimento/clientes/cadastrar", cadastro, HttpStatusCode.Created);
        var clienteId = ObterGuid(clienteCriado, "id");

        var clienteConsultado = await GetJsonAsync($"/api/v1/atendimento/clientes/consultar/{clienteId}");
        var clientePorDocumento = await GetJsonAsync($"/api/v1/atendimento/clientes/consultar-por-documento/{cadastro.Documento}");
        var clientes = await GetJsonAsync("/api/v1/atendimento/clientes/listar");

        var atualizacao = ClienteRequestBuilder.Novo()
            .ComNome("Maria Cliente Atualizada")
            .ComTelefone("(11) 98888-7777")
            .ComEmail("maria.atualizada@email.com")
            .BuildAtualizacao(clienteId);
        var clienteAtualizado = await PutJsonAsync($"/api/v1/atendimento/clientes/{clienteId}/atualizar", atualizacao);

        await DeleteAsync($"/api/v1/atendimento/clientes/{clienteId}/remover");
        var consultaAposRemocao = await Client.GetAsync($"/api/v1/atendimento/clientes/consultar/{clienteId}");

        // Assert
        ObterString(clienteConsultado, "nome").Should().Be(cadastro.Nome);
        ObterGuid(clientePorDocumento, "id").Should().Be(clienteId);
        ListagemDevePossuirItens(clientes);
        ObterString(clienteAtualizado, "nome").Should().Be(atualizacao.Nome);
        consultaAposRemocao.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [RequiresDockerFact]
    public async Task Dado_ClienteComMultiplosCamposInvalidos_Quando_Cadastrar_Entao_DeveRetornarTodosErrosDeValidacao()
    {
        // Arrange
        var cadastro = ClienteRequestBuilder.Novo()
            .ComDocumento(string.Empty)
            .ComNome(string.Empty)
            .ComTelefone(string.Empty)
            .ComEmail(string.Empty)
            .BuildCadastro();

        // Act
        var response = await PostJsonAsync("/api/v1/atendimento/clientes/cadastrar", cadastro, HttpStatusCode.BadRequest);

        // Assert
        response.GetProperty("tipo").GetString().Should().Be(TipoErro.Validacao.ToString());
        response.GetProperty("erros").EnumerateArray()
            .Select(erro => erro.GetString())
            .Should()
            .BeEquivalentTo(ClienteValidationMessages.DocumentoObrigatorio, ClienteValidationMessages.NomeObrigatorio, ClienteValidationMessages.TelefoneObrigatorio, ClienteValidationMessages.EmailObrigatorio);
    }
}

