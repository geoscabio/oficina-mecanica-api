using System.Net;
using FluentAssertions;
using OficinaMecanica.API.IntegrationTests.Atendimento.Builders;
using OficinaMecanica.API.IntegrationTests.Fixtures;

namespace OficinaMecanica.API.IntegrationTests.Atendimento.Controllers;

[Collection(OficinaMecanicaApiCollection.Nome)]
public sealed class VeiculosControllerTests : ApiIntegrationTestBase
{
    public VeiculosControllerTests(OficinaMecanicaApiFixture fixture)
        : base(fixture)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await AutenticarComoAdministradorAsync();
    }

    [RequiresDockerFact]
    public async Task Dado_VeiculoValido_Quando_ExecutarCrud_Entao_DevePersistirAlterarListarERemover()
    {
        // Arrange
        var clienteId = await CadastrarClienteAsync();
        var cadastro = VeiculoRequestBuilder.Novo()
            .ComClienteId(clienteId)
            .BuildCadastro();

        // Act
        var veiculoCriado = await PostJsonAsync("/api/v1/atendimento/veiculos/cadastrar", cadastro, HttpStatusCode.Created);
        var veiculoId = ObterGuid(veiculoCriado, "id");

        var veiculoConsultado = await GetJsonAsync($"/api/v1/atendimento/veiculos/consultar/{veiculoId}");
        var veiculoPorPlaca = await GetJsonAsync($"/api/v1/atendimento/veiculos/consultar-por-placa/{cadastro.Placa}");
        var veiculos = await GetJsonAsync("/api/v1/atendimento/veiculos/listar");

        var atualizacao = VeiculoRequestBuilder.Novo()
            .ComClienteId(clienteId)
            .ComPlaca("XYZ-9876")
            .ComModelo("Palio")
            .BuildAtualizacao(veiculoId);
        var veiculoAtualizado = await PutJsonAsync($"/api/v1/atendimento/veiculos/{veiculoId}/atualizar", atualizacao);

        await DeleteAsync($"/api/v1/atendimento/veiculos/{veiculoId}/remover");
        var consultaAposRemocao = await Client.GetAsync($"/api/v1/atendimento/veiculos/consultar/{veiculoId}");

        // Assert
        ObterString(veiculoConsultado, "modelo").Should().Be(cadastro.Modelo);
        ObterGuid(veiculoPorPlaca, "id").Should().Be(veiculoId);
        ListagemDevePossuirItens(veiculos);
        ObterString(veiculoAtualizado, "modelo").Should().Be(atualizacao.Modelo);
        consultaAposRemocao.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<Guid> CadastrarClienteAsync()
    {
        var response = await PostJsonAsync("/api/v1/atendimento/clientes/cadastrar", ClienteRequestBuilder.Novo().BuildCadastro(), HttpStatusCode.Created);

        return ObterGuid(response, "id");
    }
}

