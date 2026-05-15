using FluentAssertions;
using OficinaMecanica.Domain.GestaoOrdemServico.Entities;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.UnitTests.GestaoOrdemServico.Factories;

namespace OficinaMecanica.Domain.UnitTests.GestaoOrdemServico.Entities;

public class PecaInsumoTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_CriarPecaInsumo_Entao_DeveRegistrarQuantidadeValorEValorTotal()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();

        // Act
        var pecaInsumo = PecaInsumo.Criar(
            pecaInsumoCatalogoId,
            OrdemServicoTestDataFactory.QuantidadePecaInsumoPadrao,
            OrdemServicoTestDataFactory.ValorPecaInsumoPadrao);

        // Assert
        pecaInsumo.Id.Should().NotBeEmpty();
        pecaInsumo.PecaInsumoCatalogoId.Should().Be(pecaInsumoCatalogoId);
        pecaInsumo.Quantidade.Should().Be(OrdemServicoTestDataFactory.QuantidadePecaInsumoPadrao);
        pecaInsumo.ValorUnitario.Should().Be(OrdemServicoTestDataFactory.ValorPecaInsumoPadrao);
        pecaInsumo.ValorTotal.Should().Be(90m);
    }

    [Fact]
    public void Dado_PecaInsumoCatalogoIdVazio_Quando_CriarPecaInsumo_Entao_DeveLancarDomainException()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.Empty;

        // Act
        var acao = () => PecaInsumo.Criar(
            pecaInsumoCatalogoId,
            OrdemServicoTestDataFactory.QuantidadePecaInsumoPadrao,
            OrdemServicoTestDataFactory.ValorPecaInsumoPadrao);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.PecaInsumoCatalogoObrigatorio);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Dado_QuantidadeInvalida_Quando_CriarPecaInsumo_Entao_DeveLancarDomainException(
        int quantidade)
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();

        // Act
        var acao = () => PecaInsumo.Criar(
            pecaInsumoCatalogoId,
            quantidade,
            OrdemServicoTestDataFactory.ValorPecaInsumoPadrao);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.QuantidadePecaInsumoMaiorQueZero);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Dado_ValorUnitarioInvalido_Quando_CriarPecaInsumo_Entao_DeveLancarDomainException(
        decimal valorUnitario)
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();

        // Act
        var acao = () => PecaInsumo.Criar(
            pecaInsumoCatalogoId,
            OrdemServicoTestDataFactory.QuantidadePecaInsumoPadrao,
            valorUnitario);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.ValorUnitarioPecaInsumoMaiorQueZero);
    }
}