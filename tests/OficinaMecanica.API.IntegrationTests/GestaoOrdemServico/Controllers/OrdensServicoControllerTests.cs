using System.Net;
using FluentAssertions;
using OficinaMecanica.API.IntegrationTests.Administrativo.Builders;
using OficinaMecanica.API.IntegrationTests.Atendimento.Builders;
using OficinaMecanica.API.IntegrationTests.Fixtures;
using OficinaMecanica.API.IntegrationTests.GestaoOrdemServico.Builders;

namespace OficinaMecanica.API.IntegrationTests.GestaoOrdemServico.Controllers;

[Collection(OficinaMecanicaApiCollection.Nome)]
public sealed class OrdensServicoControllerTests : ApiIntegrationTestBase
{
    public OrdensServicoControllerTests(OficinaMecanicaApiFixture fixture)
        : base(fixture)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await AutenticarComoAdministradorAsync();
    }

    [RequiresDockerFact]
    public async Task Dado_DadosValidos_Quando_AbrirConsultarListarECancelar_Entao_DevePersistirOrdemServico()
    {
        // Arrange
        var veiculoId = await CadastrarVeiculoAsync();
        var mecanicoId = await CadastrarMecanicoAsync();
        var request = OrdemServicoRequestBuilder.Novo()
            .ComVeiculoId(veiculoId)
            .ComMecanicoId(mecanicoId)
            .BuildAbertura();

        // Act
        var ordemCriada = await PostJsonAsync(
            "/api/v1/gestao-ordem-servico/ordens-servico/cadastrar",
            request,
            HttpStatusCode.Created);
        var ordemServicoId = ObterGuid(ordemCriada, "id");

        var ordemConsultada = await GetJsonAsync(
            $"/api/v1/gestao-ordem-servico/ordens-servico/consultar/{ordemServicoId}");
        var status = await GetJsonAsync(
            $"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/consultar-status");
        var ordens = await GetJsonAsync("/api/v1/gestao-ordem-servico/ordens-servico/listar");

        var cancelamento = OrdemServicoRequestBuilder.Novo().BuildCancelamento(ordemServicoId);
        var ordemCancelada = await PostJsonAsync(
            $"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/cancelar",
            cancelamento,
            HttpStatusCode.OK);

        // Assert
        ObterGuid(ordemConsultada, "id").Should().Be(ordemServicoId);
        ObterString(status, "status").Should().Be("Recebida");
        ListagemDevePossuirItens(ordens);
        ObterString(ordemCancelada, "status").Should().Be("Cancelada");
    }

    private async Task<Guid> CadastrarVeiculoAsync()
    {
        var clienteId = await CadastrarClienteAsync();
        var response = await PostJsonAsync(
            "/api/v1/atendimento/veiculos/cadastrar",
            VeiculoRequestBuilder.Novo()
                .ComClienteId(clienteId)
                .BuildCadastro(),
            HttpStatusCode.Created);

        return ObterGuid(response, "id");
    }

    private async Task<Guid> CadastrarClienteAsync()
    {
        var response = await PostJsonAsync(
            "/api/v1/atendimento/clientes/cadastrar",
            ClienteRequestBuilder.Novo().BuildCadastro(),
            HttpStatusCode.Created);

        return ObterGuid(response, "id");
    }

    private async Task<Guid> CadastrarMecanicoAsync()
    {
        var response = await PostJsonAsync(
            "/api/v1/administrativo/mecanicos/cadastrar",
            MecanicoRequestBuilder.Novo().BuildCadastro(),
            HttpStatusCode.Created);

        return ObterGuid(response, "id");
    }
}


