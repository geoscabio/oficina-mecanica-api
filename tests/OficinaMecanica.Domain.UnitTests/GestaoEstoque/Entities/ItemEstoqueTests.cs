using FluentAssertions;
using OficinaMecanica.Domain.GestaoEstoque.Entities;
using OficinaMecanica.Domain.GestaoEstoque.Messages;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.UnitTests.GestaoEstoque.Factories;

namespace OficinaMecanica.Domain.UnitTests.GestaoEstoque.Entities;

public class ItemEstoqueTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_CriarItemEstoque_Entao_DeveRegistrarDisponibilidadeInicial()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();
        const int quantidadeDisponivel = EstoqueTestDataFactory.QuantidadeDisponivelPadrao;

        // Act
        var item = ItemEstoque.Criar(
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
        var acao = () => ItemEstoque.Criar(
            pecaInsumoCatalogoId,
            quantidadeDisponivel);

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
        var acao = () => ItemEstoque.Criar(
            pecaInsumoCatalogoId,
            quantidadeDisponivel);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(EstoqueErrorMessages.QuantidadeDisponivelNaoNegativa);
    }

    [Fact]
    public void Dado_ItemComQuantidadeDisponivel_Quando_PossuiDisponibilidade_Entao_DeveRetornarVerdadeiro()
    {
        // Arrange
        var item = EstoqueTestDataFactory.CriarItemEstoquePadrao();

        // Act
        var possuiDisponibilidade = item.PossuiDisponibilidade(
            EstoqueTestDataFactory.QuantidadeReservadaPadrao);

        // Assert
        possuiDisponibilidade.Should().BeTrue();
    }

    [Fact]
    public void Dado_ItemSemQuantidadeDisponivelSuficiente_Quando_PossuiDisponibilidade_Entao_DeveRetornarFalso()
    {
        // Arrange
        var item = EstoqueTestDataFactory.CriarItemEstoquePadrao(
            quantidadeDisponivel: 3);

        // Act
        var possuiDisponibilidade = item.PossuiDisponibilidade(
            EstoqueTestDataFactory.QuantidadeReservadaPadrao);

        // Assert
        possuiDisponibilidade.Should().BeFalse();
    }

    [Fact]
    public void Dado_QuantidadeInvalida_Quando_PossuiDisponibilidade_Entao_DeveLancarDomainException()
    {
        // Arrange
        var item = EstoqueTestDataFactory.CriarItemEstoquePadrao();

        // Act
        var acao = () => item.PossuiDisponibilidade(0);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(EstoqueErrorMessages.QuantidadeMaiorQueZero);
    }

    [Fact]
    public void Dado_ItemComQuantidadeDisponivel_Quando_Reservar_Entao_DeveMoverQuantidadeParaReservada()
    {
        // Arrange
        var item = EstoqueTestDataFactory.CriarItemEstoquePadrao();

        // Act
        item.Reservar(EstoqueTestDataFactory.QuantidadeReservadaPadrao);

        // Assert
        item.QuantidadeDisponivel.Should().Be(6);
        item.QuantidadeReservada.Should().Be(EstoqueTestDataFactory.QuantidadeReservadaPadrao);
    }

    [Fact]
    public void Dado_ItemSemQuantidadeDisponivelSuficiente_Quando_Reservar_Entao_DeveLancarDomainException()
    {
        // Arrange
        var item = EstoqueTestDataFactory.CriarItemEstoquePadrao(
            quantidadeDisponivel: 3);

        // Act
        var acao = () => item.Reservar(EstoqueTestDataFactory.QuantidadeReservadaPadrao);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(EstoqueErrorMessages.EstoqueInsuficiente);
    }

    [Fact]
    public void Dado_QuantidadeInvalida_Quando_Reservar_Entao_DeveLancarDomainException()
    {
        // Arrange
        var item = EstoqueTestDataFactory.CriarItemEstoquePadrao();

        // Act
        var acao = () => item.Reservar(0);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(EstoqueErrorMessages.QuantidadeMaiorQueZero);
    }

    [Fact]
    public void Dado_ItemComQuantidadeReservada_Quando_Estornar_Entao_DeveRetornarQuantidadeParaDisponivel()
    {
        // Arrange
        var item = EstoqueTestDataFactory.CriarItemEstoqueComReserva();

        // Act
        item.Estornar(2);

        // Assert
        item.QuantidadeDisponivel.Should().Be(8);
        item.QuantidadeReservada.Should().Be(2);
    }

    [Fact]
    public void Dado_ItemSemQuantidadeReservadaSuficiente_Quando_Estornar_Entao_DeveLancarDomainException()
    {
        // Arrange
        var item = EstoqueTestDataFactory.CriarItemEstoqueComReserva(
            quantidadeReservada: 2);

        // Act
        var acao = () => item.Estornar(EstoqueTestDataFactory.QuantidadeReservadaPadrao);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(EstoqueErrorMessages.QuantidadeReservadaInsuficiente);
    }

    [Fact]
    public void Dado_QuantidadeInvalida_Quando_Estornar_Entao_DeveLancarDomainException()
    {
        // Arrange
        var item = EstoqueTestDataFactory.CriarItemEstoqueComReserva();

        // Act
        var acao = () => item.Estornar(0);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(EstoqueErrorMessages.QuantidadeMaiorQueZero);
    }

    [Fact]
    public void Dado_ItemComQuantidadeReservada_Quando_Baixar_Entao_DeveReduzirQuantidadeReservada()
    {
        // Arrange
        var item = EstoqueTestDataFactory.CriarItemEstoqueComReserva();

        // Act
        item.Baixar(3);

        // Assert
        item.QuantidadeDisponivel.Should().Be(6);
        item.QuantidadeReservada.Should().Be(1);
    }

    [Fact]
    public void Dado_ItemSemQuantidadeReservadaSuficiente_Quando_Baixar_Entao_DeveLancarDomainException()
    {
        // Arrange
        var item = EstoqueTestDataFactory.CriarItemEstoqueComReserva(
            quantidadeReservada: 2);

        // Act
        var acao = () => item.Baixar(EstoqueTestDataFactory.QuantidadeReservadaPadrao);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(EstoqueErrorMessages.QuantidadeReservadaInsuficiente);
    }

    [Fact]
    public void Dado_QuantidadeInvalida_Quando_Baixar_Entao_DeveLancarDomainException()
    {
        // Arrange
        var item = EstoqueTestDataFactory.CriarItemEstoqueComReserva();

        // Act
        var acao = () => item.Baixar(0);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(EstoqueErrorMessages.QuantidadeMaiorQueZero);
    }

    [Fact]
    public void Dado_QuantidadeValida_Quando_RegistrarEntrada_Entao_DeveSomarQuantidadeDisponivel()
    {
        // Arrange
        var item = EstoqueTestDataFactory.CriarItemEstoquePadrao();

        // Act
        item.RegistrarEntrada(EstoqueTestDataFactory.QuantidadeEntradaPadrao);

        // Assert
        item.QuantidadeDisponivel.Should().Be(15);
        item.QuantidadeReservada.Should().Be(0);
    }

    [Fact]
    public void Dado_QuantidadeInvalida_Quando_RegistrarEntrada_Entao_DeveLancarDomainException()
    {
        // Arrange
        var item = EstoqueTestDataFactory.CriarItemEstoquePadrao();

        // Act
        var acao = () => item.RegistrarEntrada(0);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(EstoqueErrorMessages.QuantidadeMaiorQueZero);
    }

    [Fact]
    public void Dado_QuantidadeDisponivelValida_Quando_AtualizarQuantidadeDisponivel_Entao_DeveAtualizarQuantidade()
    {
        // Arrange
        var item = EstoqueTestDataFactory.CriarItemEstoquePadrao();

        // Act
        item.AtualizarQuantidadeDisponivel(
            EstoqueTestDataFactory.QuantidadeAtualizadaPadrao);

        // Assert
        item.QuantidadeDisponivel.Should().Be(EstoqueTestDataFactory.QuantidadeAtualizadaPadrao);
        item.QuantidadeReservada.Should().Be(0);
    }

    [Fact]
    public void Dado_QuantidadeDisponivelNegativa_Quando_AtualizarQuantidadeDisponivel_Entao_DeveLancarDomainException()
    {
        // Arrange
        var item = EstoqueTestDataFactory.CriarItemEstoquePadrao();

        // Act
        var acao = () => item.AtualizarQuantidadeDisponivel(-1);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(EstoqueErrorMessages.QuantidadeDisponivelNaoNegativa);
    }
}