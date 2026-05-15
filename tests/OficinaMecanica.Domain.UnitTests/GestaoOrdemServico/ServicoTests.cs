using FluentAssertions;
using OficinaMecanica.Domain.GestaoOrdemServico.Enums;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.UnitTests.GestaoOrdemServico.Factories;

namespace OficinaMecanica.Domain.UnitTests.GestaoOrdemServico;

public class ServicoTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_CriarServico_Entao_DeveRegistrarServicoPendenteDeExecucao()
    {
        // Arrange
        var servicoCatalogoId = Guid.NewGuid();

        // Act
        var servico = OrdemServicoTestDataFactory.CriarServicoPadrao(servicoCatalogoId);

        // Assert
        servico.Id.Should().NotBeEmpty();
        servico.ServicoCatalogoId.Should().Be(servicoCatalogoId);
        servico.Valor.Should().Be(OrdemServicoTestDataFactory.ValorServicoPadrao);
        servico.Status.Should().Be(StatusServico.PENDENTE);
        servico.DataInicio.Should().BeNull();
        servico.DataFim.Should().BeNull();
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
}
