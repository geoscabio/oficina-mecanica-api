using FluentAssertions;
using OficinaMecanica.Domain.Estoque.Entities;
using OficinaMecanica.Domain.Estoque.Exceptions;

namespace OficinaMecanica.Domain.UnitTests.Estoque;

public class ItemEstoqueTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_CriarItemEstoque_Entao_DeveRegistrarDisponibilidadeInicial()
    {
        var pecaInsumoCatalogoId = Guid.NewGuid();

        var item = ItemEstoque.Criar(pecaInsumoCatalogoId, 10);

        item.Id.Should().NotBeEmpty();
        item.PecaInsumoCatalogoId.Should().Be(pecaInsumoCatalogoId);
        item.QuantidadeDisponivel.Should().Be(10);
        item.QuantidadeReservada.Should().Be(0);
    }

    [Fact]
    public void Dado_CatalogoIdVazio_Quando_CriarItemEstoque_Entao_DeveLancarItemEstoqueInvalidoException()
    {
        var acao = () => ItemEstoque.Criar(Guid.Empty, 10);

        acao.Should().Throw<ItemEstoqueInvalidoException>();
    }

    [Fact]
    public void Dado_QuantidadeInicialNegativa_Quando_CriarItemEstoque_Entao_DeveLancarItemEstoqueInvalidoException()
    {
        var acao = () => ItemEstoque.Criar(Guid.NewGuid(), -1);

        acao.Should().Throw<ItemEstoqueInvalidoException>();
    }

    [Fact]
    public void Dado_ItemComQuantidadeDisponivel_Quando_Reservar_Entao_DeveMoverQuantidadeParaReservada()
    {
        var item = ItemEstoque.Criar(Guid.NewGuid(), 10);

        item.Reservar(4);

        item.QuantidadeDisponivel.Should().Be(6);
        item.QuantidadeReservada.Should().Be(4);
    }

    [Fact]
    public void Dado_ItemSemQuantidadeDisponivelSuficiente_Quando_Reservar_Entao_DeveLancarEstoqueInsuficienteException()
    {
        var item = ItemEstoque.Criar(Guid.NewGuid(), 3);

        var acao = () => item.Reservar(4);

        acao.Should().Throw<EstoqueInsuficienteException>();
    }

    [Fact]
    public void Dado_ItemComQuantidadeReservada_Quando_Estornar_Entao_DeveRetornarQuantidadeParaDisponivel()
    {
        var item = ItemEstoque.Criar(Guid.NewGuid(), 10);
        item.Reservar(4);

        item.Estornar(2);

        item.QuantidadeDisponivel.Should().Be(8);
        item.QuantidadeReservada.Should().Be(2);
    }

    [Fact]
    public void Dado_ItemComQuantidadeReservada_Quando_Baixar_Entao_DeveReduzirQuantidadeReservada()
    {
        var item = ItemEstoque.Criar(Guid.NewGuid(), 10);
        item.Reservar(4);

        item.Baixar(3);

        item.QuantidadeDisponivel.Should().Be(6);
        item.QuantidadeReservada.Should().Be(1);
    }
}
