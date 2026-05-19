using System.Net;
using System.Text.Json;
using FluentAssertions;
using OficinaMecanica.API.IntegrationTests.Administrativo.Builders;
using OficinaMecanica.API.IntegrationTests.Atendimento.Builders;
using OficinaMecanica.API.IntegrationTests.Fixtures;
using OficinaMecanica.API.IntegrationTests.GestaoEstoque.Builders;
using OficinaMecanica.API.IntegrationTests.GestaoOrdemServico.Builders;

namespace OficinaMecanica.API.IntegrationTests.GestaoOrdemServico.Scenarios;

[Collection(OficinaMecanicaApiCollection.Nome)]
public sealed class AtendimentoOrdemServicoScenarioTests : ApiIntegrationTestBase
{
    public AtendimentoOrdemServicoScenarioTests(OficinaMecanicaApiFixture fixture)
        : base(fixture)
    {
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await AutenticarComoAdministradorAsync();
    }

    [Fact]
    public async Task Dado_DadosValidos_Quando_ExecutarAtendimentoCompleto_Entao_DeveEntregarOrdemServicoECalcularTempoMedio()
    {
        // Arrange
        var clienteId = await CadastrarClienteAsync();
        var veiculoId = await CadastrarVeiculoAsync(clienteId);
        var mecanicoId = await CadastrarMecanicoAsync();
        var servicoCatalogoId = await CadastrarServicoCatalogoAsync();
        var pecaInsumoCatalogoId = await CadastrarPecaInsumoCatalogoAsync();

        await RegistrarEntradaEstoqueAsync(pecaInsumoCatalogoId);

        var abertura = OrdemServicoRequestBuilder.Novo()
            .ComVeiculoId(veiculoId)
            .ComMecanicoId(mecanicoId)
            .BuildAbertura();

        var ordemServico = await PostJsonAsync(
            "/api/v1/gestao-ordem-servico/ordens-servico/cadastrar",
            abertura,
            HttpStatusCode.Created);
        var ordemServicoId = ObterGuid(ordemServico, "id");

        // Act
        var statusRecebida = await GetJsonAsync(
            $"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/consultar-status");

        await PostJsonAsync(
            $"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/iniciar-diagnostico",
            request: null);

        var definicaoServicos = OrdemServicoRequestBuilder.Novo()
            .ComServicoCatalogoId(servicoCatalogoId)
            .BuildDefinicaoServicos(ordemServicoId);
        await PutJsonAsync(
            $"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/definir-servicos",
            definicaoServicos);

        var statusComServico = await GetJsonAsync(
            $"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/consultar-status");
        var servicoId = statusComServico
            .GetProperty("servicos")
            .EnumerateArray()
            .Single()
            .GetProperty("servicoId")
            .GetGuid();

        var reservaPecas = OrdemServicoRequestBuilder.Novo()
            .ComPecaInsumo(pecaInsumoCatalogoId, quantidade: 1)
            .BuildReservaPecasInsumos(ordemServicoId);
        await PostJsonAsync(
            $"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/reservar-pecas-insumos",
            reservaPecas);

        await PostJsonAsync(
            $"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/aguardar-aprovacao",
            request: null);

        await PostJsonAsync(
            $"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/iniciar-execucao",
            request: null);

        await PostJsonAsync(
            $"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/servicos/{servicoId}/iniciar-execucao",
            request: null);

        await PostJsonAsync(
            $"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/servicos/{servicoId}/finalizar",
            request: null);

        var ordemFinalizada = await PostJsonAsync(
            $"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/finalizar",
            request: null);

        var ordemEntregue = await PostJsonAsync(
            $"/api/v1/gestao-ordem-servico/ordens-servico/{ordemServicoId}/entregar",
            request: null);

        var tempoMedio = await GetJsonAsync(
            $"/api/v1/gestao-ordem-servico/tempo-medio-servicos/consultar/{servicoCatalogoId}");
        var temposMedios = await GetJsonAsync("/api/v1/gestao-ordem-servico/tempo-medio-servicos/listar");
        var ordensServico = await GetJsonAsync("/api/v1/gestao-ordem-servico/ordens-servico/listar");

        // Assert
        ObterString(statusRecebida, "status").Should().Be("RECEBIDA");
        ObterString(statusComServico, "status").Should().Be("EM_DIAGNOSTICO");
        ObterString(ordemFinalizada, "status").Should().Be("FINALIZADA");
        ObterString(ordemEntregue, "status").Should().Be("ENTREGUE");
        tempoMedio.GetProperty("tempoMedioExecucaoEmMinutos").ValueKind.Should().NotBe(JsonValueKind.Null);
        ListagemDevePossuirItens(temposMedios);
        ListagemDevePossuirItens(ordensServico);
    }

    private async Task<Guid> CadastrarClienteAsync()
    {
        var response = await PostJsonAsync(
            "/api/v1/atendimento/clientes/cadastrar",
            ClienteRequestBuilder.Novo().BuildCadastro(),
            HttpStatusCode.Created);

        return ObterGuid(response, "id");
    }

    private async Task<Guid> CadastrarVeiculoAsync(Guid clienteId)
    {
        var response = await PostJsonAsync(
            "/api/v1/atendimento/veiculos/cadastrar",
            VeiculoRequestBuilder.Novo()
                .ComClienteId(clienteId)
                .BuildCadastro(),
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

    private async Task<Guid> CadastrarServicoCatalogoAsync()
    {
        var response = await PostJsonAsync(
            "/api/v1/administrativo/servicos-catalogo/cadastrar",
            ServicoCatalogoRequestBuilder.Novo().BuildCadastro(),
            HttpStatusCode.Created);

        return ObterGuid(response, "id");
    }

    private async Task<Guid> CadastrarPecaInsumoCatalogoAsync()
    {
        var response = await PostJsonAsync(
            "/api/v1/administrativo/pecas-insumos-catalogo/cadastrar",
            PecaInsumoCatalogoRequestBuilder.Novo().BuildCadastro(),
            HttpStatusCode.Created);

        return ObterGuid(response, "id");
    }

    private async Task RegistrarEntradaEstoqueAsync(Guid pecaInsumoCatalogoId)
    {
        await PostJsonAsync(
            "/api/v1/gestao-estoque/estoque/registrar-entrada",
            EstoqueRequestBuilder.Novo()
                .ComPecaInsumoCatalogoId(pecaInsumoCatalogoId)
                .BuildRegistroEntrada(),
            HttpStatusCode.Created);
    }
}
