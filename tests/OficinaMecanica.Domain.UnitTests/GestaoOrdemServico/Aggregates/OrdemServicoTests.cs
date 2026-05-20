using FluentAssertions;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Enums;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.UnitTests.GestaoOrdemServico.Factories;

namespace OficinaMecanica.Domain.UnitTests.GestaoOrdemServico.Aggregates;

public class OrdemServicoTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_AbrirOrdemServico_Entao_DeveFicarRecebida()
    {
        // Arrange
        var veiculoId = Guid.NewGuid();
        var mecanicoId = Guid.NewGuid();

        // Act
        var ordemServico = OrdemServico.Abrir(OrdemServicoTestDataFactory.NumeroPadrao, veiculoId, mecanicoId);

        // Assert
        ordemServico.Id.Should().NotBeEmpty();
        ordemServico.Numero.Should().Be(OrdemServicoTestDataFactory.NumeroPadrao);
        ordemServico.VeiculoId.Should().Be(veiculoId);
        ordemServico.MecanicoId.Should().Be(mecanicoId);
        ordemServico.Status.Should().Be(StatusOrdemServico.Recebida);
        ordemServico.DataInicio.Should().NotBeNull();
        ordemServico.DataFim.Should().BeNull();
        ordemServico.MotivoCancelamento.Should().BeNull();
        ordemServico.ValorTotal.Should().Be(0m);
        ordemServico.Servicos.Should().BeEmpty();
        ordemServico.PecasInsumos.Should().BeEmpty();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Dado_NumeroInvalido_Quando_AbrirOrdemServico_Entao_DeveLancarDomainException(int numero)
    {
        // Arrange
        var veiculoId = Guid.NewGuid();
        var mecanicoId = Guid.NewGuid();

        // Act
        var acao = () => OrdemServico.Abrir(numero, veiculoId, mecanicoId);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.NumeroObrigatorio);
    }

    [Fact]
    public void Dado_VeiculoIdVazio_Quando_AbrirOrdemServico_Entao_DeveLancarDomainException()
    {
        // Arrange
        var mecanicoId = Guid.NewGuid();

        // Act
        var acao = () => OrdemServico.Abrir(OrdemServicoTestDataFactory.NumeroPadrao, Guid.Empty, mecanicoId);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.VeiculoObrigatorio);
    }

    [Fact]
    public void Dado_MecanicoIdVazio_Quando_AbrirOrdemServico_Entao_DeveLancarDomainException()
    {
        // Arrange
        var veiculoId = Guid.NewGuid();

        // Act
        var acao = () => OrdemServico.Abrir(OrdemServicoTestDataFactory.NumeroPadrao, veiculoId, Guid.Empty);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.MecanicoObrigatorio);
    }

    [Fact]
    public void Dado_OrdemServicoRecebida_Quando_IniciarDiagnostico_Entao_DeveFicarEmDiagnostico()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoPadrao();

        // Act
        ordemServico.IniciarDiagnostico();

        // Assert
        ordemServico.Status.Should().Be(StatusOrdemServico.EmDiagnostico);
        ordemServico.DataFim.Should().BeNull();
    }

    [Fact]
    public void Dado_OrdemServicoEmDiagnostico_Quando_IniciarDiagnostico_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnostico();

        // Act
        var acao = () => ordemServico.IniciarDiagnostico();

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.TransicaoStatusInvalida);
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
        ordemServico.Servicos.Single().ServicoCatalogoId.Should().Be(servicoCatalogoId);
        ordemServico.Servicos.Single().Valor.Should().Be(OrdemServicoTestDataFactory.ValorServicoPadrao);
        ordemServico.Servicos.Single().Status.Should().Be(StatusServico.Pendente);
        ordemServico.ValorTotal.Should().Be(OrdemServicoTestDataFactory.ValorServicoPadrao);
    }

    [Fact]
    public void Dado_OrdemServicoRecebida_Quando_DefinirServico_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoPadrao();

        // Act
        var acao = () => ordemServico.DefinirServico(Guid.NewGuid(), OrdemServicoTestDataFactory.ValorServicoPadrao);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.TransicaoStatusInvalida);
    }

    [Fact]
    public void Dado_ServicoCatalogoIdVazio_Quando_DefinirServico_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnostico();

        // Act
        var acao = () => ordemServico.DefinirServico(Guid.Empty, OrdemServicoTestDataFactory.ValorServicoPadrao);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.ServicoCatalogoObrigatorio);
    }

    [Fact]
    public void Dado_ValorServicoInvalido_Quando_DefinirServico_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnostico();

        // Act
        var acao = () => ordemServico.DefinirServico(Guid.NewGuid(), 0m);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.ValorServicoMaiorQueZero);
    }

    [Fact]
    public void Dado_OrdemServicoEmDiagnostico_Quando_ReservarPecaInsumo_Entao_DeveAdicionarPecaInsumoEAtualizarOrcamento()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnostico();
        var pecaInsumoCatalogoId = Guid.NewGuid();

        // Act
        ordemServico.ReservarPecaInsumo(pecaInsumoCatalogoId, OrdemServicoTestDataFactory.QuantidadePecaInsumoPadrao, OrdemServicoTestDataFactory.ValorPecaInsumoPadrao);

        // Assert
        ordemServico.PecasInsumos.Should().ContainSingle();
        ordemServico.PecasInsumos.Single().PecaInsumoCatalogoId.Should().Be(pecaInsumoCatalogoId);
        ordemServico.PecasInsumos.Single().Quantidade.Should().Be(OrdemServicoTestDataFactory.QuantidadePecaInsumoPadrao);
        ordemServico.PecasInsumos.Single().ValorUnitario.Should().Be(OrdemServicoTestDataFactory.ValorPecaInsumoPadrao);
        ordemServico.PecasInsumos.Single().ValorTotal.Should().Be(90m);
        ordemServico.ValorTotal.Should().Be(90m);
    }

    [Fact]
    public void Dado_OrdemServicoRecebida_Quando_ReservarPecaInsumo_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoPadrao();

        // Act
        var acao = () => ordemServico.ReservarPecaInsumo(Guid.NewGuid(), OrdemServicoTestDataFactory.QuantidadePecaInsumoPadrao, OrdemServicoTestDataFactory.ValorPecaInsumoPadrao);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.TransicaoStatusInvalida);
    }

    [Fact]
    public void Dado_OrdemServicoComServicoEPecaInsumo_Quando_CalcularOrcamento_Entao_DeveSomarServicosEPecasInsumos()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnosticoComServicoEPecaInsumo();

        // Act
        ordemServico.CalcularOrcamento();

        // Assert
        ordemServico.ValorTotal.Should().Be(240m);
    }

    [Fact]
    public void Dado_OrdemServicoEmDiagnosticoComServicoDefinido_Quando_AguardarAprovacao_Entao_DeveFicarAguardandoAprovacao()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnosticoComServico();

        // Act
        ordemServico.AguardarAprovacao();

        // Assert
        ordemServico.Status.Should().Be(StatusOrdemServico.AguardandoAprovacao);
        ordemServico.ValorTotal.Should().Be(OrdemServicoTestDataFactory.ValorServicoPadrao);
        ordemServico.DataFim.Should().BeNull();
    }

    [Fact]
    public void Dado_OrdemServicoEmDiagnosticoSemServico_Quando_AguardarAprovacao_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnostico();

        // Act
        var acao = () => ordemServico.AguardarAprovacao();

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.ServicoObrigatorioParaAguardarAprovacao);
    }

    [Fact]
    public void Dado_OrdemServicoRecebida_Quando_AguardarAprovacao_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoPadrao();

        // Act
        var acao = () => ordemServico.AguardarAprovacao();

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.TransicaoStatusInvalida);
    }

    [Fact]
    public void Dado_OrdemServicoAguardandoAprovacao_Quando_IniciarExecucao_Entao_DeveFicarEmExecucao()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoAguardandoAprovacao();

        // Act
        ordemServico.IniciarExecucao();

        // Assert
        ordemServico.Status.Should().Be(StatusOrdemServico.EmExecucao);
    }

    [Fact]
    public void Dado_OrdemServicoRecebida_Quando_IniciarExecucao_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoPadrao();

        // Act
        var acao = () => ordemServico.IniciarExecucao();

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.TransicaoStatusInvalida);
    }

    [Fact]
    public void Dado_OrdemServicoEmExecucaoComServicoPendente_Quando_IniciarExecucaoServico_Entao_DeveFicarServicoEmExecucao()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmExecucao();
        var servicoId = ordemServico.Servicos.Single().Id;

        // Act
        ordemServico.IniciarExecucaoServico(servicoId);

        // Assert
        ordemServico.Servicos.Single().Status.Should().Be(StatusServico.EmExecucao);
        ordemServico.Servicos.Single().DataInicio.Should().NotBeNull();
        ordemServico.Servicos.Single().DataFim.Should().BeNull();
    }

    [Fact]
    public void Dado_OrdemServicoAguardandoAprovacao_Quando_IniciarExecucaoServico_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoAguardandoAprovacao();
        var servicoId = ordemServico.Servicos.Single().Id;

        // Act
        var acao = () => ordemServico.IniciarExecucaoServico(servicoId);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.TransicaoStatusInvalida);
    }

    [Fact]
    public void Dado_ServicoInexistente_Quando_IniciarExecucaoServico_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmExecucao();

        // Act
        var acao = () => ordemServico.IniciarExecucaoServico(Guid.NewGuid());

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.ServicoNaoEncontrado);
    }

    [Fact]
    public void Dado_OrdemServicoEmExecucaoComServicoEmExecucao_Quando_FinalizarServico_Entao_DeveFicarServicoFinalizado()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmExecucaoComServicoEmExecucao();
        var servicoId = ordemServico.Servicos.Single().Id;

        // Act
        ordemServico.FinalizarServico(servicoId);

        // Assert
        ordemServico.Servicos.Single().Status.Should().Be(StatusServico.Finalizado);
        ordemServico.Servicos.Single().DataInicio.Should().NotBeNull();
        ordemServico.Servicos.Single().DataFim.Should().NotBeNull();
    }

    [Fact]
    public void Dado_OrdemServicoAguardandoAprovacao_Quando_FinalizarServico_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoAguardandoAprovacao();
        var servicoId = ordemServico.Servicos.Single().Id;

        // Act
        var acao = () => ordemServico.FinalizarServico(servicoId);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.TransicaoStatusInvalida);
    }

    [Fact]
    public void Dado_ServicoInexistente_Quando_FinalizarServico_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmExecucao();

        // Act
        var acao = () => ordemServico.FinalizarServico(Guid.NewGuid());

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.ServicoNaoEncontrado);
    }

    [Fact]
    public void Dado_OrdemServicoEmExecucaoComTodosServicosFinalizados_Quando_Finalizar_Entao_DeveFicarFinalizada()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmExecucaoComServicoFinalizado();

        // Act
        ordemServico.Finalizar();

        // Assert
        ordemServico.Status.Should().Be(StatusOrdemServico.Finalizada);
        ordemServico.DataFim.Should().NotBeNull();
    }

    [Fact]
    public void Dado_OrdemServicoEmExecucaoComServicoPendente_Quando_Finalizar_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmExecucao();

        // Act
        var acao = () => ordemServico.Finalizar();

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.ServicosFinalizadosObrigatorios);
    }

    [Fact]
    public void Dado_OrdemServicoEmExecucaoSemServicos_Quando_Finalizar_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoAguardandoAprovacao();

        ordemServico.IniciarExecucao();

        // Act
        var acao = () => ordemServico.Finalizar();

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.ServicosFinalizadosObrigatorios);
    }

    [Fact]
    public void Dado_OrdemServicoAguardandoAprovacao_Quando_Finalizar_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoAguardandoAprovacao();

        // Act
        var acao = () => ordemServico.Finalizar();

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.TransicaoStatusInvalida);
    }

    [Fact]
    public void Dado_OrdemServicoFinalizada_Quando_Entregar_Entao_DeveFicarEntregue()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoFinalizada();

        // Act
        ordemServico.Entregar();

        // Assert
        ordemServico.Status.Should().Be(StatusOrdemServico.Entregue);
    }

    [Fact]
    public void Dado_OrdemServicoEmExecucao_Quando_Entregar_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmExecucaoComServicoFinalizado();

        // Act
        var acao = () => ordemServico.Entregar();

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.TransicaoStatusInvalida);
    }

    [Fact]
    public void Dado_OrdemServicoAguardandoAprovacao_Quando_Cancelar_Entao_DeveFicarCancelada()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoAguardandoAprovacao();

        // Act
        ordemServico.Cancelar(MotivoCancelamentoOrdemServico.ReprovacaoOrcamento);

        // Assert
        ordemServico.Status.Should().Be(StatusOrdemServico.Cancelada);
        ordemServico.MotivoCancelamento.Should().Be(MotivoCancelamentoOrdemServico.ReprovacaoOrcamento);
        ordemServico.DataFim.Should().NotBeNull();
    }

    [Fact]
    public void Dado_OrdemServicoRecebida_Quando_Cancelar_Entao_DeveFicarCancelada()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoPadrao();

        // Act
        ordemServico.Cancelar(MotivoCancelamentoOrdemServico.EstoqueInsuficiente);

        // Assert
        ordemServico.Status.Should().Be(StatusOrdemServico.Cancelada);
        ordemServico.MotivoCancelamento.Should().Be(MotivoCancelamentoOrdemServico.EstoqueInsuficiente);
        ordemServico.DataFim.Should().NotBeNull();
    }

    [Fact]
    public void Dado_MotivoInvalido_Quando_Cancelar_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoAguardandoAprovacao();

        // Act
        var acao = () => ordemServico.Cancelar((MotivoCancelamentoOrdemServico)99);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.MotivoCancelamentoInvalido);
    }

    [Fact]
    public void Dado_OrdemServicoFinalizada_Quando_Cancelar_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoFinalizada();

        // Act
        var acao = () => ordemServico.Cancelar(MotivoCancelamentoOrdemServico.ReprovacaoOrcamento);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.CancelamentoStatusInvalido);
    }

    [Fact]
    public void Dado_OrdemServicoEntregue_Quando_Cancelar_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEntregue();

        // Act
        var acao = () => ordemServico.Cancelar(MotivoCancelamentoOrdemServico.ReprovacaoOrcamento);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.CancelamentoStatusInvalido);
    }

    [Fact]
    public void Dado_OrdemServicoCancelada_Quando_CancelarNovamente_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoAguardandoAprovacao();

        ordemServico.Cancelar(MotivoCancelamentoOrdemServico.ReprovacaoOrcamento);

        // Act
        var acao = () => ordemServico.Cancelar(MotivoCancelamentoOrdemServico.EstoqueInsuficiente);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.CancelamentoStatusInvalido);
    }
}
