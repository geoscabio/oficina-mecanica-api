using FluentAssertions;
using OficinaMecanica.Domain.GestaoOrdemServico.Entities;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.UnitTests.GestaoOrdemServico.Factories;

namespace OficinaMecanica.Domain.UnitTests.GestaoOrdemServico;

public class PecaInsumoTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_CriarPecaInsumo_Entao_DeveRegistrarQuantidadeEValor()
    {
        // Arrange
        var catalogoId = Guid.NewGuid();

        // Act
        var pecaInsumo = OrdemServicoTestDataFactory.CriarPecaInsumoPadrao(catalogoId);

        // Assert
        pecaInsumo.Id.Should().NotBeEmpty();
        pecaInsumo.PecaInsumoCatalogoId.Should().Be(catalogoId);
        pecaInsumo.Quantidade.Should().Be(OrdemServicoTestDataFactory.QuantidadePecaInsumoPadrao);
        pecaInsumo.ValorUnitario.Should().Be(OrdemServicoTestDataFactory.ValorPecaInsumoPadrao);
        pecaInsumo.ValorTotal.Should().Be(90m);
    }

    [Fact]
    public void Dado_QuantidadeInvalida_Quando_CriarPecaInsumo_Entao_DeveLancarDomainException()
    {
        // Arrange
        var catalogoId = Guid.NewGuid();
        const int quantidade = 0;
        const decimal valorUnitario = OrdemServicoTestDataFactory.ValorPecaInsumoPadrao;

        // Act
        var acao = () => PecaInsumo.Criar(catalogoId, quantidade, valorUnitario);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.QuantidadePecaInsumoMaiorQueZero);
    }

    [Fact]
    public void Dado_ValorUnitarioInvalido_Quando_CriarPecaInsumo_Entao_DeveLancarDomainException()
    {
        // Arrange
        var catalogoId = Guid.NewGuid();
        const int quantidade = OrdemServicoTestDataFactory.QuantidadePecaInsumoPadrao;
        const decimal valorUnitario = 0m;

        // Act
        var acao = () => PecaInsumo.Criar(catalogoId, quantidade, valorUnitario);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.ValorUnitarioPecaInsumoMaiorQueZero);
    }
}
