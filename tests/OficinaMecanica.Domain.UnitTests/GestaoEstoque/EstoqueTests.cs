using FluentAssertions;
using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Entities;
using OficinaMecanica.Domain.GestaoEstoque.Messages;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.UnitTests.GestaoEstoque.Builders;

namespace OficinaMecanica.Domain.UnitTests.GestaoEstoque;

public class EstoqueTests
{
    [Fact]
    public void Dado_ItemValido_Quando_CriarEstoque_Entao_DeveRegistrarEstoqueComItem()
    {
        // Arrange
        var item = EstoqueTestDataFactory.CriarItemEstoquePadrao();

        // Act
        var estoque = Estoque.Criar(new[] { item });

        // Assert
        estoque.Id.Should().NotBeEmpty();
        estoque.ItensEstoque.Should().ContainSingle().Which.Should().Be(item);
    }

    [Fact]
    public void Dado_ListaSemItens_Quando_CriarEstoque_Entao_DeveLancarDomainException()
    {
        // Arrange
        var itens = Array.Empty<ItemEstoque>();

        // Act
        var acao = () => Estoque.Criar(itens);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(EstoqueErrorMessages.EstoqueSemItens);
    }

    [Fact]
    public void Dado_ItemExistenteComDisponibilidade_Quando_VerificarDisponibilidade_Entao_DeveRetornarVerdadeiro()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();
        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao(pecaInsumoCatalogoId);

        // Act
        var disponivel = estoque.VerificarDisponibilidade(pecaInsumoCatalogoId, 5);

        // Assert
        disponivel.Should().BeTrue();
    }

    [Fact]
    public void Dado_ItemExistenteSemDisponibilidade_Quando_VerificarDisponibilidade_Entao_DeveRetornarFalso()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();
        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao(
            pecaInsumoCatalogoId,
            quantidadeDisponivel: 3);

        // Act
        var disponivel = estoque.VerificarDisponibilidade(pecaInsumoCatalogoId, 5);

        // Assert
        disponivel.Should().BeFalse();
    }

    [Fact]
    public void Dado_ItemExistenteComDisponibilidade_Quando_ReservarItens_Entao_DeveReservarQuantidade()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();
        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao(pecaInsumoCatalogoId);

        // Act
        estoque.ReservarItens(pecaInsumoCatalogoId, 4);

        // Assert
        var item = estoque.ObterItem(pecaInsumoCatalogoId);
        item.QuantidadeDisponivel.Should().Be(6);
        item.QuantidadeReservada.Should().Be(4);
    }

    [Fact]
    public void Dado_ItemExistenteComReserva_Quando_EstornarItens_Entao_DeveEstornarQuantidadeReservada()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();
        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao(pecaInsumoCatalogoId);
        estoque.ReservarItens(pecaInsumoCatalogoId, 4);

        // Act
        estoque.EstornarItens(pecaInsumoCatalogoId, 4);

        // Assert
        var item = estoque.ObterItem(pecaInsumoCatalogoId);
        item.QuantidadeDisponivel.Should().Be(10);
        item.QuantidadeReservada.Should().Be(0);
    }

    [Fact]
    public void Dado_ItemExistenteComReserva_Quando_BaixarItens_Entao_DeveBaixarQuantidadeReservada()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();
        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao(pecaInsumoCatalogoId);
        estoque.ReservarItens(pecaInsumoCatalogoId, 4);

        // Act
        estoque.BaixarItens(pecaInsumoCatalogoId, 4);

        // Assert
        var item = estoque.ObterItem(pecaInsumoCatalogoId);
        item.QuantidadeDisponivel.Should().Be(6);
        item.QuantidadeReservada.Should().Be(0);
    }

    [Fact]
    public void Dado_ItemInexistente_Quando_ReservarItens_Entao_DeveLancarDomainException()
    {
        // Arrange
        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao();
        var pecaInsumoCatalogoIdInexistente = Guid.NewGuid();

        // Act
        var acao = () => estoque.ReservarItens(pecaInsumoCatalogoIdInexistente, 1);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(EstoqueErrorMessages.ItemNaoEncontrado);
    }
}
