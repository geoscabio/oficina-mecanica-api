using FluentAssertions;
using OficinaMecanica.Domain.GestaoEstoque.Entities;
using OficinaMecanica.Domain.GestaoEstoque.Messages;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.UnitTests.GestaoEstoque.Builders;

namespace OficinaMecanica.Domain.UnitTests.GestaoEstoque;

public class ItemEstoqueTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_CriarItemEstoque_Entao_DeveRegistrarDisponibilidadeInicial()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();
        const int quantidadeDisponivel = EstoqueTestDataFactory.QuantidadeDisponivelPadrao;

        // Act
        var item = EstoqueTestDataFactory.CriarItemEstoquePadrao(
            pecaInsumoCatalogoId,
            quantidadeDisponivel);

        // Assert
        item.Id.Should().NotBeEmpty();
        item.PecaInsumoCatalogoId.Should().Be(pecaInsumoCatalogoId);
        item.QuantidadeDisponivel.Should().Be(quantidadeDisponivel);
        item.QuantidadeReservada.Should().Be(0);
    }

    [Fact]
    public void Dado_CatalogoIdVazio_Quando_CriarItemEstoque_Entao_DeveLancarDomainException()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.Empty;
        const int quantidadeDisponivel = EstoqueTestDataFactory.QuantidadeDisponivelPadrao;

        // Act
        var acao = () => ItemEstoque.Criar(pecaInsumoCatalogoId, quantidadeDisponivel);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(EstoqueErrorMessages.PecaInsumoCatalogoObrigatorio);
    }

    [Fact]
    public void Dado_QuantidadeInicialNegativa_Quando_CriarItemEstoque_Entao_DeveLancarDomainException()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();
        const int quantidadeDisponivel = -1;

        // Act
        var acao = () => ItemEstoque.Criar(pecaInsumoCatalogoId, quantidadeDisponivel);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(EstoqueErrorMessages.QuantidadeDisponivelNaoNegativa);
    }

    [Fact]
    public void Dado_ItemComQuantidadeDisponivel_Quando_Reservar_Entao_DeveMoverQuantidadeParaReservada()
    {
        // Arrange
        var item = EstoqueTestDataFactory.CriarItemEstoquePadrao();

        // Act
        item.Reservar(4);

        // Assert
        item.QuantidadeDisponivel.Should().Be(6);
        item.QuantidadeReservada.Should().Be(4);
    }

    [Fact]
    public void Dado_ItemSemQuantidadeDisponivelSuficiente_Quando_Reservar_Entao_DeveLancarDomainException()
    {
        // Arrange
        var item = EstoqueTestDataFactory.CriarItemEstoquePadrao(quantidadeDisponivel: 3);

        // Act
        var acao = () => item.Reservar(4);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(EstoqueErrorMessages.EstoqueInsuficiente);
    }

    [Fact]
    public void Dado_ItemComQuantidadeReservada_Quando_Estornar_Entao_DeveRetornarQuantidadeParaDisponivel()
    {
        // Arrange
        var item = EstoqueTestDataFactory.CriarItemEstoquePadrao();
        item.Reservar(4);

        // Act
        item.Estornar(2);

        // Assert
        item.QuantidadeDisponivel.Should().Be(8);
        item.QuantidadeReservada.Should().Be(2);
    }

    [Fact]
    public void Dado_ItemComQuantidadeReservada_Quando_Baixar_Entao_DeveReduzirQuantidadeReservada()
    {
        // Arrange
        var item = EstoqueTestDataFactory.CriarItemEstoquePadrao();
        item.Reservar(4);

        // Act
        item.Baixar(3);

        // Assert
        item.QuantidadeDisponivel.Should().Be(6);
        item.QuantidadeReservada.Should().Be(1);
    }
}
