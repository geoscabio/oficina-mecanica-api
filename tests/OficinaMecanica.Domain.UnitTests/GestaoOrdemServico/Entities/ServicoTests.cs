using FluentAssertions;
using OficinaMecanica.Domain.GestaoOrdemServico.Entities;
using OficinaMecanica.Domain.GestaoOrdemServico.Enums;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.UnitTests.GestaoOrdemServico.Factories;

namespace OficinaMecanica.Domain.UnitTests.GestaoOrdemServico.Entities;

public class ServicoTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_CriarServico_Entao_DeveRegistrarServicoPendenteDeExecucao()
    {
        // Arrange
        var servicoCatalogoId = Guid.NewGuid();

        // Act
        var servico = Servico.Criar(
            servicoCatalogoId,
            OrdemServicoTestDataFactory.ValorServicoPadrao);

        // Assert
        servico.Id.Should().NotBeEmpty();
        servico.ServicoCatalogoId.Should().Be(servicoCatalogoId);
        servico.Valor.Should().Be(OrdemServicoTestDataFactory.ValorServicoPadrao);
        servico.Status.Should().Be(StatusServico.PENDENTE);
        servico.DataInicio.Should().BeNull();
        servico.DataFim.Should().BeNull();
    }

    [Fact]
    public void Dado_ServicoCatalogoIdVazio_Quando_CriarServico_Entao_DeveLancarDomainException()
    {
        // Arrange
        var servicoCatalogoId = Guid.Empty;

        // Act
        var acao = () => Servico.Criar(
            servicoCatalogoId,
            OrdemServicoTestDataFactory.ValorServicoPadrao);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.ServicoCatalogoObrigatorio);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Dado_ValorInvalido_Quando_CriarServico_Entao_DeveLancarDomainException(decimal valor)
    {
        // Arrange
        var servicoCatalogoId = Guid.NewGuid();

        // Act
        var acao = () => Servico.Criar(
            servicoCatalogoId,
            valor);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.ValorServicoMaiorQueZero);
    }

    [Fact]
    public void Dado_ServicoPendente_Quando_IniciarExecucao_Entao_DeveFicarEmExecucao()
    {
        // Arrange
        var servico = OrdemServicoTestDataFactory.CriarServicoPadrao();

        // Act
        servico.IniciarExecucao();

        // Assert
        servico.Status.Should().Be(StatusServico.EM_EXECUCAO);
        servico.DataInicio.Should().NotBeNull();
        servico.DataFim.Should().BeNull();
    }

    [Fact]
    public void Dado_ServicoEmExecucao_Quando_IniciarExecucao_Entao_DeveLancarDomainException()
    {
        // Arrange
        var servico = OrdemServicoTestDataFactory.CriarServicoEmExecucao();

        // Act
        var acao = servico.IniciarExecucao;

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.ServicoPendenteParaIniciarExecucao);
    }

    [Fact]
    public void Dado_ServicoFinalizado_Quando_IniciarExecucao_Entao_DeveLancarDomainException()
    {
        // Arrange
        var servico = OrdemServicoTestDataFactory.CriarServicoFinalizado();

        // Act
        var acao = servico.IniciarExecucao;

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.ServicoPendenteParaIniciarExecucao);
    }

    [Fact]
    public void Dado_ServicoEmExecucao_Quando_Finalizar_Entao_DeveFicarFinalizado()
    {
        // Arrange
        var servico = OrdemServicoTestDataFactory.CriarServicoEmExecucao();

        // Act
        servico.Finalizar();

        // Assert
        servico.Status.Should().Be(StatusServico.FINALIZADO);
        servico.DataInicio.Should().NotBeNull();
        servico.DataFim.Should().NotBeNull();
    }

    [Fact]
    public void Dado_ServicoPendente_Quando_Finalizar_Entao_DeveLancarDomainException()
    {
        // Arrange
        var servico = OrdemServicoTestDataFactory.CriarServicoPadrao();

        // Act
        var acao = servico.Finalizar;

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.ServicoEmExecucaoParaFinalizar);
    }

    [Fact]
    public void Dado_ServicoFinalizado_Quando_Finalizar_Entao_DeveLancarDomainException()
    {
        // Arrange
        var servico = OrdemServicoTestDataFactory.CriarServicoFinalizado();

        // Act
        var acao = servico.Finalizar;

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.ServicoEmExecucaoParaFinalizar);
    }
}