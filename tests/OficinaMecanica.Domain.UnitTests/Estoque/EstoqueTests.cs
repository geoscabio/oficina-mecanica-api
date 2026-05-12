using FluentAssertions;
using OficinaMecanica.Domain.Estoque.Entities;
using OficinaMecanica.Domain.Estoque.Exceptions;
using EstoqueAggregate = OficinaMecanica.Domain.Estoque.Aggregates.Estoque;

namespace OficinaMecanica.Domain.UnitTests.Estoque;

public class EstoqueTests
{
    [Fact]
    public void Dado_ItemValido_Quando_CriarEstoque_Entao_DeveRegistrarEstoqueComItem()
    {
        var item = ItemEstoque.Criar(Guid.NewGuid(), 10);

        var estoque = EstoqueAggregate.Criar(new[] { item });

        estoque.Id.Should().NotBeEmpty();
        estoque.ItensEstoque.Should().ContainSingle().Which.Should().Be(item);
    }

    [Fact]
    public void Dado_ListaSemItens_Quando_CriarEstoque_Entao_DeveLancarEstoqueInvalidoException()
    {
        var acao = () => EstoqueAggregate.Criar(Array.Empty<ItemEstoque>());

        acao.Should().Throw<EstoqueInvalidoException>();
    }

    [Fact]
    public void Dado_ItemExistenteComDisponibilidade_Quando_VerificarDisponibilidade_Entao_DeveRetornarVerdadeiro()
    {
        var pecaInsumoCatalogoId = Guid.NewGuid();
        var estoque = EstoqueAggregate.Criar(new[] { ItemEstoque.Criar(pecaInsumoCatalogoId, 10) });

        var disponivel = estoque.VerificarDisponibilidade(pecaInsumoCatalogoId, 5);

        disponivel.Should().BeTrue();
    }

    [Fact]
    public void Dado_ItemExistenteSemDisponibilidade_Quando_VerificarDisponibilidade_Entao_DeveRetornarFalso()
    {
        var pecaInsumoCatalogoId = Guid.NewGuid();
        var estoque = EstoqueAggregate.Criar(new[] { ItemEstoque.Criar(pecaInsumoCatalogoId, 3) });

        var disponivel = estoque.VerificarDisponibilidade(pecaInsumoCatalogoId, 5);

        disponivel.Should().BeFalse();
    }

    [Fact]
    public void Dado_ItemExistenteComDisponibilidade_Quando_ReservarItens_Entao_DeveReservarQuantidade()
    {
        var pecaInsumoCatalogoId = Guid.NewGuid();
        var estoque = EstoqueAggregate.Criar(new[] { ItemEstoque.Criar(pecaInsumoCatalogoId, 10) });

        estoque.ReservarItens(pecaInsumoCatalogoId, 4);

        var item = estoque.ObterItem(pecaInsumoCatalogoId);
        item.QuantidadeDisponivel.Should().Be(6);
        item.QuantidadeReservada.Should().Be(4);
    }

    [Fact]
    public void Dado_ItemExistenteComReserva_Quando_EstornarItens_Entao_DeveEstornarQuantidadeReservada()
    {
        var pecaInsumoCatalogoId = Guid.NewGuid();
        var estoque = EstoqueAggregate.Criar(new[] { ItemEstoque.Criar(pecaInsumoCatalogoId, 10) });
        estoque.ReservarItens(pecaInsumoCatalogoId, 4);

        estoque.EstornarItens(pecaInsumoCatalogoId, 4);

        var item = estoque.ObterItem(pecaInsumoCatalogoId);
        item.QuantidadeDisponivel.Should().Be(10);
        item.QuantidadeReservada.Should().Be(0);
    }

    [Fact]
    public void Dado_ItemExistenteComReserva_Quando_BaixarItens_Entao_DeveBaixarQuantidadeReservada()
    {
        var pecaInsumoCatalogoId = Guid.NewGuid();
        var estoque = EstoqueAggregate.Criar(new[] { ItemEstoque.Criar(pecaInsumoCatalogoId, 10) });
        estoque.ReservarItens(pecaInsumoCatalogoId, 4);

        estoque.BaixarItens(pecaInsumoCatalogoId, 4);

        var item = estoque.ObterItem(pecaInsumoCatalogoId);
        item.QuantidadeDisponivel.Should().Be(6);
        item.QuantidadeReservada.Should().Be(0);
    }

    [Fact]
    public void Dado_ItemInexistente_Quando_ReservarItens_Entao_DeveLancarEstoqueInvalidoException()
    {
        var estoque = EstoqueAggregate.Criar(new[] { ItemEstoque.Criar(Guid.NewGuid(), 10) });

        var acao = () => estoque.ReservarItens(Guid.NewGuid(), 1);

        acao.Should().Throw<EstoqueInvalidoException>();
    }
}
