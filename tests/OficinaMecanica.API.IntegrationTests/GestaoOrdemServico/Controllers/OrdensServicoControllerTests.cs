using System.Net;
using System.Text.Json;
using FluentAssertions;
using OficinaMecanica.API.IntegrationTests.Administrativo.Builders;
using OficinaMecanica.API.IntegrationTests.Atendimento.Builders;
using OficinaMecanica.API.IntegrationTests.Fixtures;
using OficinaMecanica.API.IntegrationTests.GestaoEstoque.Builders;
using OficinaMecanica.API.IntegrationTests.GestaoOrdemServico.Builders;
using OficinaMecanica.Domain.GestaoOrdemServico.Enums;

namespace OficinaMecanica.API.IntegrationTests.GestaoOrdemServico.Controllers;

[Collection(OficinaMecanicaApiCollection.Nome)]
public sealed class OrdensServicoControllerTests : ApiIntegrationTestBase
{
    private static readonly string[] StatusHistoricoEsperados = ["Finalizada", "Entregue", "Cancelada"];

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
        var clienteId = await CadastrarClienteAsync();
        var veiculoId = await CadastrarVeiculoAsync(clienteId, "CAD-1001");
        var mecanicoId = await CadastrarMecanicoAsync();
        var request = OrdemServicoRequestBuilder.Novo()
            .ComClienteId(clienteId)
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
    public async Task Dado_PayloadOficialComServicosEPecas_Quando_Abrir_Entao_DeveRegistrarOrcamentoInicial()
    {
        // Arrange
        var clienteId = await CadastrarClienteAsync();
        var veiculoId = await CadastrarVeiculoAsync(clienteId, "ABR-2001");
        var mecanicoId = await CadastrarMecanicoAsync();
        var servicoCatalogoId = await CadastrarServicoCatalogoAsync();
        var pecaInsumoCatalogoId = await CadastrarPecaInsumoCatalogoAsync();

        await RegistrarEntradaEstoqueAsync(pecaInsumoCatalogoId);

        var request = OrdemServicoRequestBuilder.Novo()
            .ComClienteId(clienteId)
            .ComVeiculoId(veiculoId)
            .ComMecanicoId(mecanicoId)
            .ComServicoCatalogoId(servicoCatalogoId)
            .ComPecaInsumo(pecaInsumoCatalogoId, quantidade: 1)
            .BuildAbertura();

        // Act
        var response = await PostJsonAsync("/api/v1/gestao-ordem-servico/ordens-servico/cadastrar", request, HttpStatusCode.Created);

        // Assert
        ObterGuid(response, "id").Should().NotBeEmpty();
        ObterString(response, "status").Should().Be("Recebida");
        response.GetProperty("valorTotal").GetDecimal().Should().BeGreaterThan(0);
        response.GetProperty("servicos").EnumerateArray().Should().ContainSingle();
        response.GetProperty("pecasInsumos").EnumerateArray().Should().ContainSingle();
    }

    [RequiresDockerFact]
    public async Task Dado_OrdensServicoComStatusDiversos_Quando_Listar_Entao_DeveRetornarFilaOficialEHistoricoSeparado()
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
        var ordens = await GetJsonAsync("/api/v1/gestao-ordem-servico/ordens-servico/listar?tamanhoPagina=10");
        var ordensHistorico = await GetJsonAsync("/api/v1/gestao-ordem-servico/ordens-servico/listar-historico?tamanhoPagina=10");
        var itens = ordens.GetProperty("itens").EnumerateArray().ToArray();
        var itensHistorico = ordensHistorico.GetProperty("itens").EnumerateArray().ToArray();

        // Assert
        ordens.GetProperty("totalItens").GetInt32().Should().Be(5);
        itens.Select(item => ObterString(item, "status")).Should().Equal(
            "EmExecucao",
            "AguardandoAprovacao",
            "EmDiagnostico",
            "Recebida",
            "Recebida");

        itens.Select(item => ObterGuid(item, "id")).Should().Equal(
            ordemEmExecucaoId,
            ordemAguardandoAprovacaoId,
            ordemEmDiagnosticoId,
            ordemRecebidaAntigaId,
            ordemRecebidaNovaId);

        ordensHistorico.GetProperty("totalItens").GetInt32().Should().Be(8);
        itensHistorico.Select(item => ObterString(item, "status")).Should().Contain(StatusHistoricoEsperados);
    }

    [RequiresDockerFact]
    public async Task Dado_OrdemServicoAguardandoAprovacao_Quando_NotificarAprovacaoOrcamento_Entao_DeveIniciarExecucao()
    {
        var clienteId = await CadastrarClienteAsync();
        var mecanicoId = await CadastrarMecanicoAsync();
        var servicoCatalogoId = await CadastrarServicoCatalogoAsync();
        var ordemServicoId = await CriarOrdemServicoAguardandoAprovacaoAsync(servicoCatalogoId, clienteId, mecanicoId, "NTF-1001");
        var request = OrdemServicoRequestBuilder.Novo().BuildNotificacaoOrcamento(ordemServicoId, DecisaoOrcamento.Aprovado);

        AutenticarWebhookOrcamento();

        var response = await PostJsonAsync($"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/orcamento/notificacoes", request);

        ObterGuid(response, "id").Should().Be(ordemServicoId);
        ObterString(response, "status").Should().Be("EmExecucao");
    }

    [RequiresDockerFact]
    public async Task Dado_OrdemServicoAguardandoAprovacao_Quando_NotificarRecusaOrcamento_Entao_DeveCancelar()
    {
        var clienteId = await CadastrarClienteAsync();
        var mecanicoId = await CadastrarMecanicoAsync();
        var servicoCatalogoId = await CadastrarServicoCatalogoAsync();
        var ordemServicoId = await CriarOrdemServicoAguardandoAprovacaoAsync(servicoCatalogoId, clienteId, mecanicoId, "NTF-1002");
        var request = OrdemServicoRequestBuilder.Novo().BuildNotificacaoOrcamento(ordemServicoId, DecisaoOrcamento.Recusado);

        AutenticarWebhookOrcamento();

        var response = await PostJsonAsync($"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/orcamento/notificacoes", request);

        ObterGuid(response, "id").Should().Be(ordemServicoId);
        ObterString(response, "status").Should().Be("Cancelada");
    }

    [RequiresDockerFact]
    public async Task Dado_OrdemServicoRecebida_Quando_NotificarAprovacaoOrcamento_Entao_DeveRetornarRegraNegocio()
    {
        var clienteId = await CadastrarClienteAsync();
        var mecanicoId = await CadastrarMecanicoAsync();
        var ordemServicoId = await CriarOrdemServicoRecebidaAsync(clienteId, mecanicoId, "NTF-1003");
        var request = OrdemServicoRequestBuilder.Novo().BuildNotificacaoOrcamento(ordemServicoId, DecisaoOrcamento.Aprovado);

        AutenticarWebhookOrcamento();

        var response = await PostJsonAsync($"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/orcamento/notificacoes", request, HttpStatusCode.UnprocessableEntity);

        ObterString(response, "tipo").Should().Be("RegraNegocio");
    }

    [RequiresDockerFact]
    public async Task Dado_OrdemServicoExistente_Quando_ConsultarStatusSemAutenticacao_Entao_DeveRetornarStatus()
    {
        var clienteId = await CadastrarClienteAsync();
        var mecanicoId = await CadastrarMecanicoAsync();
        var ordemServicoId = await CriarOrdemServicoRecebidaAsync(clienteId, mecanicoId, "NTF-1004");

        Client.DefaultRequestHeaders.Authorization = null;

        var response = await GetJsonAsync($"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/consultar-status");

        ObterGuid(response, "ordemServicoId").Should().Be(ordemServicoId);
        ObterString(response, "status").Should().Be("Recebida");
    }

    [RequiresDockerFact]
    public async Task Dado_NotificacaoOrcamentoSemToken_Quando_NotificarDecisao_Entao_DeveRetornarNaoAutorizado()
    {
        var clienteId = await CadastrarClienteAsync();
        var mecanicoId = await CadastrarMecanicoAsync();
        var servicoCatalogoId = await CadastrarServicoCatalogoAsync();
        var ordemServicoId = await CriarOrdemServicoAguardandoAprovacaoAsync(servicoCatalogoId, clienteId, mecanicoId, "NTF-1005");
        var request = OrdemServicoRequestBuilder.Novo().BuildNotificacaoOrcamento(ordemServicoId, DecisaoOrcamento.Aprovado);

        Client.DefaultRequestHeaders.Authorization = null;
        Client.DefaultRequestHeaders.Remove("X-Webhook-Token");

        var response = await PostJsonAsync($"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/orcamento/notificacoes", request, HttpStatusCode.Unauthorized);

        ObterString(response, "tipo").Should().Be("NaoAutorizado");
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

    private async Task<Guid> CadastrarPecaInsumoCatalogoAsync()
    {
        var response = await PostJsonAsync("/api/v1/administrativo/pecas-insumos-catalogo/cadastrar", PecaInsumoCatalogoRequestBuilder.Novo().BuildCadastro(), HttpStatusCode.Created);

        return ObterGuid(response, "id");
    }

    private async Task RegistrarEntradaEstoqueAsync(Guid pecaInsumoCatalogoId)
    {
        await PostJsonAsync("/api/v1/gestao-estoque/estoque/registrar-entrada", EstoqueRequestBuilder.Novo().ComPecaInsumoCatalogoId(pecaInsumoCatalogoId).BuildRegistroEntrada(), HttpStatusCode.Created);
    }

    private async Task<Guid> CriarOrdemServicoRecebidaAsync(Guid clienteId, Guid mecanicoId, string placa)
    {
        var veiculoId = await CadastrarVeiculoAsync(clienteId, placa);
        var request = OrdemServicoRequestBuilder.Novo()
            .ComClienteId(clienteId)
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

    private void AutenticarWebhookOrcamento()
    {
        Client.DefaultRequestHeaders.Authorization = null;
        Client.DefaultRequestHeaders.Remove("X-Webhook-Token");
        Client.DefaultRequestHeaders.Add("X-Webhook-Token", "webhook-orcamento-teste-local-2026");
    }
}


