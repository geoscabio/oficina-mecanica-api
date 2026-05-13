using FluentAssertions;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Enums;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.UnitTests.GestaoOrdemServico.Builders;

namespace OficinaMecanica.Domain.UnitTests.GestaoOrdemServico;

public class OrdemServicoTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_AbrirOrdemServico_Entao_DeveFicarRecebida()
    {
        // Arrange
        var veiculoId = Guid.NewGuid();
        var mecanicoId = Guid.NewGuid();

        // Act
        const int numero = 1;

        var ordemServico = OrdemServico.Abrir(numero, veiculoId, mecanicoId);

        // Assert
        ordemServico.Id.Should().NotBeEmpty();
        ordemServico.Numero.Should().Be(numero);
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
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoPadrao();

        // Act
        ordemServico.IniciarDiagnostico();

        // Assert
        ordemServico.Status.Should().Be(StatusOrdemServico.EM_DIAGNOSTICO);
    }

    [Fact]
    public void Dado_OrdemServicoEmDiagnostico_Quando_DefinirServico_Entao_DeveAdicionarServicoEAtualizarOrcamento()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnostico();
        var servicoCatalogoId = Guid.NewGuid();

        // Act
        ordemServico.DefinirServico(servicoCatalogoId, OrdemServicoTestDataFactory.ValorServicoPadrao);

        // Assert
        ordemServico.Servicos.Should().ContainSingle();
        ordemServico.ValorTotal.Should().Be(OrdemServicoTestDataFactory.ValorServicoPadrao);
    }

    [Fact]
    public void Dado_OrdemServicoEmDiagnostico_Quando_ReservarPecaInsumo_Entao_DeveAdicionarPecaInsumoEAtualizarOrcamento()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnostico();
        var pecaInsumoCatalogoId = Guid.NewGuid();

        // Act
        ordemServico.ReservarPecaInsumo(
            pecaInsumoCatalogoId,
            OrdemServicoTestDataFactory.QuantidadePecaInsumoPadrao,
            OrdemServicoTestDataFactory.ValorPecaInsumoPadrao);

        // Assert
        ordemServico.PecasInsumos.Should().ContainSingle();
        ordemServico.ValorTotal.Should().Be(90m);
    }

    [Fact]
    public void Dado_OrdemServicoEmDiagnosticoComServicoDefinido_Quando_AguardarAprovacao_Entao_DeveFicarAguardandoAprovacao()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnostico();
        ordemServico.DefinirServico(Guid.NewGuid(), OrdemServicoTestDataFactory.ValorServicoPadrao);

        // Act
        ordemServico.AguardarAprovacao();

        // Assert
        ordemServico.Status.Should().Be(StatusOrdemServico.AGUARDANDO_APROVACAO);
        ordemServico.ValorTotal.Should().Be(OrdemServicoTestDataFactory.ValorServicoPadrao);
    }

    [Fact]
    public void Dado_OrdemServicoAguardandoAprovacao_Quando_IniciarExecucao_Entao_DeveFicarEmExecucao()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoAguardandoAprovacao();

        // Act
        ordemServico.IniciarExecucao();

        // Assert
        ordemServico.Status.Should().Be(StatusOrdemServico.EM_EXECUCAO);
    }

    [Fact]
    public void Dado_OrdemServicoEmExecucao_Quando_IniciarEFinalizarServico_Entao_DeveFinalizarServico()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmExecucao();
        var servicoId = ordemServico.Servicos.Single().Id;

        // Act
        ordemServico.IniciarExecucaoServico(servicoId);
        ordemServico.FinalizarServico(servicoId);

        // Assert
        ordemServico.Servicos.Single().Status.Should().Be(StatusServico.FINALIZADO);
    }

    [Fact]
    public void Dado_OrdemServicoEmExecucaoComTodosServicosFinalizados_Quando_Finalizar_Entao_DeveFicarFinalizada()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmExecucao();
        var servicoId = ordemServico.Servicos.Single().Id;
        ordemServico.IniciarExecucaoServico(servicoId);
        ordemServico.FinalizarServico(servicoId);

        // Act
        ordemServico.Finalizar();

        // Assert
        ordemServico.Status.Should().Be(StatusOrdemServico.FINALIZADA);
        ordemServico.DataFim.Should().NotBeNull();
    }

    [Fact]
    public void Dado_OrdemServicoFinalizada_Quando_Entregar_Entao_DeveFicarEntregue()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoFinalizada();

        // Act
        ordemServico.Entregar();

        // Assert
        ordemServico.Status.Should().Be(StatusOrdemServico.ENTREGUE);
    }

    [Fact]
    public void Dado_OrdemServicoAguardandoAprovacao_Quando_Cancelar_Entao_DeveFicarCancelada()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoAguardandoAprovacao();

        // Act
        ordemServico.Cancelar();

        // Assert
        ordemServico.Status.Should().Be(StatusOrdemServico.CANCELADA);
        ordemServico.DataFim.Should().NotBeNull();
    }

    [Fact]
    public void Dado_OrdemServicoRecebida_Quando_IniciarExecucao_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoPadrao();

        // Act
        var acao = ordemServico.IniciarExecucao;

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.TransicaoStatusInvalida);
    }

    [Fact]
    public void Dado_OrdemServicoEmExecucaoComServicoPendente_Quando_Finalizar_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmExecucao();

        // Act
        var acao = ordemServico.Finalizar;

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.ServicosFinalizadosObrigatorios);
    }
}
