using FluentAssertions;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Enums;
using OficinaMecanica.Domain.GestaoOrdemServico.Exceptions;

namespace OficinaMecanica.Domain.UnitTests.GestaoOrdemServico;

public class OrdemServicoTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_AbrirOrdemServico_Entao_DeveFicarRecebida()
    {
        var veiculoId = Guid.NewGuid();
        var mecanicoId = Guid.NewGuid();

        var ordemServico = OrdemServico.Abrir(veiculoId, mecanicoId);

        ordemServico.Id.Should().NotBeEmpty();
        ordemServico.Numero.Should().BeGreaterThan(0);
        ordemServico.VeiculoId.Should().Be(veiculoId);
        ordemServico.MecanicoId.Should().Be(mecanicoId);
        ordemServico.Status.Should().Be(StatusOrdemServico.RECEBIDA);
        ordemServico.DataInicio.Should().NotBeNull();
        ordemServico.DataFim.Should().BeNull();
        ordemServico.ValorTotal.Should().Be(0m);
    }

    [Fact]
    public void Dado_OrdemServicoRecebida_Quando_IniciarDiagnostico_Entao_DeveFicarEmDiagnostico()
    {
        var ordemServico = CriarOrdemServico();

        ordemServico.IniciarDiagnostico();

        ordemServico.Status.Should().Be(StatusOrdemServico.EM_DIAGNOSTICO);
    }

    [Fact]
    public void Dado_OrdemServicoEmDiagnostico_Quando_DefinirServico_Entao_DeveAdicionarServicoEAtualizarOrcamento()
    {
        var ordemServico = CriarOrdemServicoEmDiagnostico();

        ordemServico.DefinirServico(Guid.NewGuid(), 150m);

        ordemServico.Servicos.Should().ContainSingle();
        ordemServico.ValorTotal.Should().Be(150m);
    }

    [Fact]
    public void Dado_OrdemServicoEmDiagnostico_Quando_ReservarPecaInsumo_Entao_DeveAdicionarPecaInsumoEAtualizarOrcamento()
    {
        var ordemServico = CriarOrdemServicoEmDiagnostico();

        ordemServico.ReservarPecaInsumo(Guid.NewGuid(), 2, 45m);

        ordemServico.PecasInsumos.Should().ContainSingle();
        ordemServico.ValorTotal.Should().Be(90m);
    }

    [Fact]
    public void Dado_OrdemServicoEmDiagnosticoComOrcamento_Quando_AguardarAprovacao_Entao_DeveFicarAguardandoAprovacao()
    {
        var ordemServico = CriarOrdemServicoEmDiagnostico();
        ordemServico.DefinirServico(Guid.NewGuid(), 150m);

        ordemServico.AguardarAprovacao();

        ordemServico.Status.Should().Be(StatusOrdemServico.AGUARDANDO_APROVACAO);
    }

    [Fact]
    public void Dado_OrdemServicoAguardandoAprovacao_Quando_IniciarExecucao_Entao_DeveFicarEmExecucao()
    {
        var ordemServico = CriarOrdemServicoAguardandoAprovacao();

        ordemServico.IniciarExecucao();

        ordemServico.Status.Should().Be(StatusOrdemServico.EM_EXECUCAO);
    }

    [Fact]
    public void Dado_OrdemServicoEmExecucao_Quando_IniciarEFinalizarServico_Entao_DeveFinalizarServico()
    {
        var ordemServico = CriarOrdemServicoEmExecucao();
        var servicoId = ordemServico.Servicos.Single().Id;

        ordemServico.IniciarExecucaoServico(servicoId);
        ordemServico.FinalizarServico(servicoId);

        ordemServico.Servicos.Single().Status.Should().Be(StatusServico.FINALIZADO);
    }

    [Fact]
    public void Dado_OrdemServicoEmExecucaoComTodosServicosFinalizados_Quando_Finalizar_Entao_DeveFicarFinalizada()
    {
        var ordemServico = CriarOrdemServicoEmExecucao();
        var servicoId = ordemServico.Servicos.Single().Id;
        ordemServico.IniciarExecucaoServico(servicoId);
        ordemServico.FinalizarServico(servicoId);

        ordemServico.Finalizar();

        ordemServico.Status.Should().Be(StatusOrdemServico.FINALIZADA);
        ordemServico.DataFim.Should().NotBeNull();
    }

    [Fact]
    public void Dado_OrdemServicoFinalizada_Quando_Entregar_Entao_DeveFicarEntregue()
    {
        var ordemServico = CriarOrdemServicoFinalizada();

        ordemServico.Entregar();

        ordemServico.Status.Should().Be(StatusOrdemServico.ENTREGUE);
    }

    [Fact]
    public void Dado_OrdemServicoAguardandoAprovacao_Quando_Cancelar_Entao_DeveFicarCancelada()
    {
        var ordemServico = CriarOrdemServicoAguardandoAprovacao();

        ordemServico.Cancelar();

        ordemServico.Status.Should().Be(StatusOrdemServico.CANCELADA);
        ordemServico.DataFim.Should().NotBeNull();
    }

    [Fact]
    public void Dado_OrdemServicoRecebida_Quando_IniciarExecucao_Entao_DeveLancarTransicaoStatusOrdemServicoInvalidaException()
    {
        var ordemServico = CriarOrdemServico();

        var acao = ordemServico.IniciarExecucao;

        acao.Should().Throw<TransicaoStatusOrdemServicoInvalidaException>();
    }

    [Fact]
    public void Dado_OrdemServicoEmExecucaoComServicoPendente_Quando_Finalizar_Entao_DeveLancarOrdemServicoInvalidaException()
    {
        var ordemServico = CriarOrdemServicoEmExecucao();

        var acao = ordemServico.Finalizar;

        acao.Should().Throw<OrdemServicoInvalidaException>();
    }

    private static OrdemServico CriarOrdemServico()
    {
        return OrdemServico.Abrir(Guid.NewGuid(), Guid.NewGuid());
    }

    private static OrdemServico CriarOrdemServicoEmDiagnostico()
    {
        var ordemServico = CriarOrdemServico();
        ordemServico.IniciarDiagnostico();
        return ordemServico;
    }

    private static OrdemServico CriarOrdemServicoAguardandoAprovacao()
    {
        var ordemServico = CriarOrdemServicoEmDiagnostico();
        ordemServico.DefinirServico(Guid.NewGuid(), 150m);
        ordemServico.AguardarAprovacao();
        return ordemServico;
    }

    private static OrdemServico CriarOrdemServicoEmExecucao()
    {
        var ordemServico = CriarOrdemServicoAguardandoAprovacao();
        ordemServico.IniciarExecucao();
        return ordemServico;
    }

    private static OrdemServico CriarOrdemServicoFinalizada()
    {
        var ordemServico = CriarOrdemServicoEmExecucao();
        var servicoId = ordemServico.Servicos.Single().Id;
        ordemServico.IniciarExecucaoServico(servicoId);
        ordemServico.FinalizarServico(servicoId);
        ordemServico.Finalizar();
        return ordemServico;
    }
}
