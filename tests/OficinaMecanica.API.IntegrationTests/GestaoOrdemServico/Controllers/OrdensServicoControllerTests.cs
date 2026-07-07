using System.Net;
using System.Text.Json;
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
        var ordemCriada = await PostJsonAsync("/api/v1/gestao-ordem-servico/ordens-servico/cadastrar", request, HttpStatusCode.Created);
        var ordemServicoId = ObterGuid(ordemCriada, "id");

        var ordemConsultada = await GetJsonAsync($"/api/v1/gestao-ordem-servico/ordens-servico/consultar/{ordemServicoId}");
        var status = await GetJsonAsync($"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/consultar-status");
        var ordens = await GetJsonAsync("/api/v1/gestao-ordem-servico/ordens-servico/listar");

        var cancelamento = OrdemServicoRequestBuilder.Novo().BuildCancelamento(ordemServicoId);
        var ordemCancelada = await PostJsonAsync($"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/cancelar", cancelamento, HttpStatusCode.OK);

        // Assert
        ObterGuid(ordemConsultada, "id").Should().Be(ordemServicoId);
        ObterString(status, "status").Should().Be("Recebida");
        ListagemDevePossuirItens(ordens);
        ObterString(ordemCancelada, "status").Should().Be("Cancelada");
    }

    [RequiresDockerFact]
    public async Task Dado_OrdensServicoComStatusDiversos_Quando_ListarAbertasETotal_Entao_DeveSepararFilaEHistorico()
    {
        // Arrange
        var clienteId = await CadastrarClienteAsync();
        var mecanicoId = await CadastrarMecanicoAsync();
        var servicoCatalogoId = await CadastrarServicoCatalogoAsync();
        var ordemRecebidaAntigaId = await CriarOrdemServicoRecebidaAsync(clienteId, mecanicoId, "LST-1001");

        await CriarOrdemServicoFinalizadaAsync(servicoCatalogoId, clienteId, mecanicoId, "LST-1002");

        var ordemAguardandoAprovacaoId = await CriarOrdemServicoAguardandoAprovacaoAsync(servicoCatalogoId, clienteId, mecanicoId, "LST-1003");

        await CriarOrdemServicoEntregueAsync(servicoCatalogoId, clienteId, mecanicoId, "LST-1004");

        var ordemEmDiagnosticoId = await CriarOrdemServicoEmDiagnosticoAsync(clienteId, mecanicoId, "LST-1005");
        var ordemRecebidaNovaId = await CriarOrdemServicoRecebidaAsync(clienteId, mecanicoId, "LST-1006");
        var ordemEmExecucaoId = await CriarOrdemServicoEmExecucaoAsync(servicoCatalogoId, clienteId, mecanicoId, "LST-1007");

        await CriarOrdemServicoCanceladaAsync(clienteId, mecanicoId, "LST-1008");

        // Act
        var ordensAbertas = await GetJsonAsync("/api/v1/gestao-ordem-servico/ordens-servico/listar-abertas?tamanhoPagina=10");
        var ordensTotais = await GetJsonAsync("/api/v1/gestao-ordem-servico/ordens-servico/listar-total?tamanhoPagina=10");
        var itensAbertos = ordensAbertas.GetProperty("itens").EnumerateArray().ToArray();
        var itensTotais = ordensTotais.GetProperty("itens").EnumerateArray().ToArray();

        // Assert
        ordensAbertas.GetProperty("totalItens").GetInt32().Should().Be(5);
        itensAbertos.Select(item => ObterString(item, "status")).Should().Equal(
            "EmExecucao",
            "AguardandoAprovacao",
            "EmDiagnostico",
            "Recebida",
            "Recebida");

        itensAbertos.Select(item => ObterGuid(item, "id")).Should().Equal(
            ordemEmExecucaoId,
            ordemAguardandoAprovacaoId,
            ordemEmDiagnosticoId,
            ordemRecebidaAntigaId,
            ordemRecebidaNovaId);

        ordensTotais.GetProperty("totalItens").GetInt32().Should().Be(8);
        itensTotais.Select(item => ObterString(item, "status")).Should().Contain(new[]
        {
            "Finalizada",
            "Entregue",
            "Cancelada"
        });
    }

    private async Task<Guid> CadastrarVeiculoAsync()
    {
        var clienteId = await CadastrarClienteAsync();
        var response = await PostJsonAsync("/api/v1/atendimento/veiculos/cadastrar", VeiculoRequestBuilder.Novo().ComClienteId(clienteId).BuildCadastro(), HttpStatusCode.Created);

        return ObterGuid(response, "id");
    }

    private async Task<Guid> CadastrarClienteAsync()
    {
        var response = await PostJsonAsync("/api/v1/atendimento/clientes/cadastrar", ClienteRequestBuilder.Novo().BuildCadastro(), HttpStatusCode.Created);

        return ObterGuid(response, "id");
    }

    private async Task<Guid> CadastrarMecanicoAsync()
    {
        var response = await PostJsonAsync("/api/v1/administrativo/mecanicos/cadastrar", MecanicoRequestBuilder.Novo().BuildCadastro(), HttpStatusCode.Created);

        return ObterGuid(response, "id");
    }

    private async Task<Guid> CadastrarVeiculoAsync(Guid clienteId, string placa)
    {
        var response = await PostJsonAsync("/api/v1/atendimento/veiculos/cadastrar", VeiculoRequestBuilder.Novo().ComClienteId(clienteId).ComPlaca(placa).BuildCadastro(), HttpStatusCode.Created);

        return ObterGuid(response, "id");
    }

    private async Task<Guid> CadastrarServicoCatalogoAsync()
    {
        var response = await PostJsonAsync("/api/v1/administrativo/servicos-catalogo/cadastrar", ServicoCatalogoRequestBuilder.Novo().BuildCadastro(), HttpStatusCode.Created);

        return ObterGuid(response, "id");
    }

    private async Task<Guid> CriarOrdemServicoRecebidaAsync(Guid clienteId, Guid mecanicoId, string placa)
    {
        var veiculoId = await CadastrarVeiculoAsync(clienteId, placa);
        var request = OrdemServicoRequestBuilder.Novo()
            .ComVeiculoId(veiculoId)
            .ComMecanicoId(mecanicoId)
            .BuildAbertura();

        var response = await PostJsonAsync("/api/v1/gestao-ordem-servico/ordens-servico/cadastrar", request, HttpStatusCode.Created);

        return ObterGuid(response, "id");
    }

    private async Task<Guid> CriarOrdemServicoEmDiagnosticoAsync(Guid clienteId, Guid mecanicoId, string placa)
    {
        var ordemServicoId = await CriarOrdemServicoRecebidaAsync(clienteId, mecanicoId, placa);

        await PostJsonAsync($"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/iniciar-diagnostico", request: null);

        return ordemServicoId;
    }

    private async Task<Guid> CriarOrdemServicoAguardandoAprovacaoAsync(Guid servicoCatalogoId, Guid clienteId, Guid mecanicoId, string placa)
    {
        var ordemServicoId = await CriarOrdemServicoEmDiagnosticoAsync(clienteId, mecanicoId, placa);
        var definicaoServicos = OrdemServicoRequestBuilder.Novo()
            .ComServicoCatalogoId(servicoCatalogoId)
            .BuildDefinicaoServicos(ordemServicoId);

        await PutJsonAsync($"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/definir-servicos", definicaoServicos);
        await PostJsonAsync($"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/aguardar-aprovacao", request: null);

        return ordemServicoId;
    }

    private async Task<Guid> CriarOrdemServicoEmExecucaoAsync(Guid servicoCatalogoId, Guid clienteId, Guid mecanicoId, string placa)
    {
        var ordemServicoId = await CriarOrdemServicoAguardandoAprovacaoAsync(servicoCatalogoId, clienteId, mecanicoId, placa);

        await PostJsonAsync($"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/iniciar-execucao", request: null);

        return ordemServicoId;
    }

    private async Task<Guid> CriarOrdemServicoFinalizadaAsync(Guid servicoCatalogoId, Guid clienteId, Guid mecanicoId, string placa)
    {
        var ordemServicoId = await CriarOrdemServicoEmExecucaoAsync(servicoCatalogoId, clienteId, mecanicoId, placa);
        var servicoId = await ObterServicoIdAsync(ordemServicoId);

        await PostJsonAsync($"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/servicos/{servicoId}/iniciar-execucao", request: null);
        await PostJsonAsync($"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/servicos/{servicoId}/finalizar", request: null);
        await PostJsonAsync($"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/finalizar", request: null);

        return ordemServicoId;
    }

    private async Task<Guid> CriarOrdemServicoEntregueAsync(Guid servicoCatalogoId, Guid clienteId, Guid mecanicoId, string placa)
    {
        var ordemServicoId = await CriarOrdemServicoFinalizadaAsync(servicoCatalogoId, clienteId, mecanicoId, placa);

        await PostJsonAsync($"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/entregar", request: null);

        return ordemServicoId;
    }

    private async Task<Guid> CriarOrdemServicoCanceladaAsync(Guid clienteId, Guid mecanicoId, string placa)
    {
        var ordemServicoId = await CriarOrdemServicoRecebidaAsync(clienteId, mecanicoId, placa);
        var cancelamento = OrdemServicoRequestBuilder.Novo().BuildCancelamento(ordemServicoId);

        await PostJsonAsync($"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/cancelar", cancelamento);

        return ordemServicoId;
    }

    private async Task<Guid> ObterServicoIdAsync(Guid ordemServicoId)
    {
        var status = await GetJsonAsync($"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/consultar-status");

        return status
            .GetProperty("servicos")
            .EnumerateArray()
            .Single()
            .GetProperty("servicoId")
            .GetGuid();
    }
}


