using FluentAssertions;
using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Entities;
using OficinaMecanica.Domain.GestaoEstoque.Messages;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.UnitTests.GestaoEstoque.Factories;

namespace OficinaMecanica.Domain.UnitTests.GestaoEstoque.Aggregates;

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
    public void Dado_ListaSemItens_Quando_CriarEstoque_Entao_DeveRegistrarEstoqueSemItens()
    {
        // Arrange
        var itens = Array.Empty<ItemEstoque>();

        // Act
        var estoque = Estoque.Criar(itens);

        // Assert
        estoque.Id.Should().NotBeEmpty();
        estoque.ItensEstoque.Should().BeEmpty();
    }

    [Fact]
    public void Dado_ItemNulo_Quando_CriarEstoque_Entao_DeveLancarDomainException()
    {
        // Arrange
        var itens = new[]
        {
            EstoqueTestDataFactory.CriarItemEstoquePadrao(),
            null!
        };

        // Act
        var acao = () => Estoque.Criar(itens);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(EstoqueErrorMessages.EstoqueComItemNulo);
    }

    [Fact]
    public void Dado_ItemExistenteComDisponibilidade_Quando_VerificarDisponibilidade_Entao_DeveRetornarVerdadeiro()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();

        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao(
            pecaInsumoCatalogoId);

        // Act
        var disponivel = estoque.VerificarDisponibilidade(
            pecaInsumoCatalogoId,
            EstoqueTestDataFactory.QuantidadeReservadaPadrao);

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
        var disponivel = estoque.VerificarDisponibilidade(
            pecaInsumoCatalogoId,
            EstoqueTestDataFactory.QuantidadeReservadaPadrao);

        // Assert
        disponivel.Should().BeFalse();
    }

    [Fact]
    public void Dado_ItemInexistente_Quando_VerificarDisponibilidade_Entao_DeveRetornarFalso()
    {
        // Arrange
        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao();

        // Act
        var disponivel = estoque.VerificarDisponibilidade(
            Guid.NewGuid(),
            EstoqueTestDataFactory.QuantidadeReservadaPadrao);

        // Assert
        disponivel.Should().BeFalse();
    }

    [Fact]
    public void Dado_CatalogoIdVazio_Quando_VerificarDisponibilidade_Entao_DeveLancarDomainException()
    {
        // Arrange
        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao();

        // Act
        var acao = () => estoque.VerificarDisponibilidade(
            Guid.Empty,
            EstoqueTestDataFactory.QuantidadeReservadaPadrao);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(EstoqueErrorMessages.PecaInsumoCatalogoObrigatorio);
    }

    [Fact]
    public void Dado_ItemExistenteComDisponibilidade_Quando_ReservarItens_Entao_DeveReservarQuantidade()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();

        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao(
            pecaInsumoCatalogoId);

        // Act
        estoque.ReservarItens(
            pecaInsumoCatalogoId,
            EstoqueTestDataFactory.QuantidadeReservadaPadrao);

        // Assert
        var item = estoque.ObterItem(pecaInsumoCatalogoId);
        item.QuantidadeDisponivel.Should().Be(6);
        item.QuantidadeReservada.Should().Be(EstoqueTestDataFactory.QuantidadeReservadaPadrao);
    }

    [Fact]
    public void Dado_ItemInexistente_Quando_ReservarItens_Entao_DeveRetornarFalha()
    {
        // Arrange
        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao();
        var pecaInsumoCatalogoIdInexistente = Guid.NewGuid();

        // Act
        var resultado = estoque.ReservarItens(
            pecaInsumoCatalogoIdInexistente,
            1);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Be(EstoqueErrorMessages.ItemNaoEncontrado);
    }

    [Fact]
    public void Dado_QuantidadeInvalida_Quando_ReservarItens_Entao_DeveRetornarFalha()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();

        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao(
            pecaInsumoCatalogoId);

        // Act
        var resultado = estoque.ReservarItens(
            pecaInsumoCatalogoId,
            0);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Be(EstoqueErrorMessages.QuantidadeMaiorQueZero);
    }

    [Fact]
    public void Dado_ItemExistenteComReserva_Quando_EstornarItens_Entao_DeveEstornarQuantidadeReservada()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();

        var estoque = EstoqueTestDataFactory.CriarEstoqueComItemReservado(
            pecaInsumoCatalogoId);

        // Act
        estoque.EstornarItens(
            pecaInsumoCatalogoId,
            EstoqueTestDataFactory.QuantidadeReservadaPadrao);

        // Assert
        var item = estoque.ObterItem(pecaInsumoCatalogoId);
        item.QuantidadeDisponivel.Should().Be(EstoqueTestDataFactory.QuantidadeDisponivelPadrao);
        item.QuantidadeReservada.Should().Be(0);
    }

    [Fact]
    public void Dado_ItemExistenteSemReservaSuficiente_Quando_EstornarItens_Entao_DeveRetornarFalha()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();

        var estoque = EstoqueTestDataFactory.CriarEstoqueComItemReservado(
            pecaInsumoCatalogoId,
            quantidadeReservada: 2);

        // Act
        var resultado = estoque.EstornarItens(
            pecaInsumoCatalogoId,
            EstoqueTestDataFactory.QuantidadeReservadaPadrao);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Be(EstoqueErrorMessages.QuantidadeReservadaInsuficiente);
    }

    [Fact]
    public void Dado_ItemExistenteComReserva_Quando_BaixarItens_Entao_DeveBaixarQuantidadeReservada()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();

        var estoque = EstoqueTestDataFactory.CriarEstoqueComItemReservado(
            pecaInsumoCatalogoId);

        // Act
        estoque.BaixarItens(
            pecaInsumoCatalogoId,
            EstoqueTestDataFactory.QuantidadeReservadaPadrao);

        // Assert
        var item = estoque.ObterItem(pecaInsumoCatalogoId);
        item.QuantidadeDisponivel.Should().Be(6);
        item.QuantidadeReservada.Should().Be(0);
    }

    [Fact]
    public void Dado_ItemExistenteSemReservaSuficiente_Quando_BaixarItens_Entao_DeveRetornarFalha()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();

        var estoque = EstoqueTestDataFactory.CriarEstoqueComItemReservado(
            pecaInsumoCatalogoId,
            quantidadeReservada: 2);

        // Act
        var resultado = estoque.BaixarItens(
            pecaInsumoCatalogoId,
            EstoqueTestDataFactory.QuantidadeReservadaPadrao);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Be(EstoqueErrorMessages.QuantidadeReservadaInsuficiente);
    }

    [Fact]
    public void Dado_ItemExistente_Quando_RegistrarEntrada_Entao_DeveSomarQuantidadeDisponivel()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();

        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao(
            pecaInsumoCatalogoId);

        // Act
        var resultado = estoque.RegistrarEntrada(
            pecaInsumoCatalogoId,
            EstoqueTestDataFactory.QuantidadeEntradaPadrao);

        // Assert
        resultado.Sucesso.Should().BeTrue();

        var item = resultado.Valor!;

        item.QuantidadeDisponivel.Should().Be(15);
        item.QuantidadeReservada.Should().Be(0);
    }

    [Fact]
    public void Dado_ItemInexistente_Quando_RegistrarEntrada_Entao_DeveAdicionarItemAoEstoque()
    {
        // Arrange
        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao();
        var pecaInsumoCatalogoId = Guid.NewGuid();

        // Act
        var resultado = estoque.RegistrarEntrada(
            pecaInsumoCatalogoId,
            EstoqueTestDataFactory.QuantidadeEntradaPadrao);

        // Assert
        resultado.Sucesso.Should().BeTrue();

        var item = resultado.Valor!;

        item.PecaInsumoCatalogoId.Should().Be(pecaInsumoCatalogoId);
        item.QuantidadeDisponivel.Should().Be(EstoqueTestDataFactory.QuantidadeEntradaPadrao);
        item.QuantidadeReservada.Should().Be(0);
        estoque.ItensEstoque.Should().Contain(item);
    }

    [Fact]
    public void Dado_QuantidadeInvalida_Quando_RegistrarEntrada_Entao_DeveRetornarFalha()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();

        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao(
            pecaInsumoCatalogoId);

        // Act
        var resultado = estoque.RegistrarEntrada(
            pecaInsumoCatalogoId,
            0);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Be(EstoqueErrorMessages.QuantidadeMaiorQueZero);
    }

    [Fact]
    public void Dado_ItemExistente_Quando_AtualizarQuantidadeDisponivel_Entao_DeveAtualizarQuantidade()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();

        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao(
            pecaInsumoCatalogoId);

        // Act
        var resultado = estoque.AtualizarQuantidadeDisponivel(
            pecaInsumoCatalogoId,
            EstoqueTestDataFactory.QuantidadeAtualizadaPadrao);

        // Assert
        resultado.Sucesso.Should().BeTrue();

        var item = resultado.Valor!;

        item.QuantidadeDisponivel.Should().Be(EstoqueTestDataFactory.QuantidadeAtualizadaPadrao);
        item.QuantidadeReservada.Should().Be(0);
    }

    [Fact]
    public void Dado_ItemInexistente_Quando_AtualizarQuantidadeDisponivel_Entao_DeveRetornarFalha()
    {
        // Arrange
        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao();

        // Act
        var resultado = estoque.AtualizarQuantidadeDisponivel(
            Guid.NewGuid(),
            EstoqueTestDataFactory.QuantidadeAtualizadaPadrao);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Mensagem.Should().Be(EstoqueErrorMessages.ItemNaoEncontrado);
    }

    [Fact]
    public void Dado_ItemExistente_Quando_ObterItem_Entao_DeveRetornarItem()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();

        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao(
            pecaInsumoCatalogoId);

        // Act
        var item = estoque.ObterItem(pecaInsumoCatalogoId);

        // Assert
        item.PecaInsumoCatalogoId.Should().Be(pecaInsumoCatalogoId);
    }

    [Fact]
    public void Dado_ItemInexistente_Quando_ObterItem_Entao_DeveLancarDomainException()
    {
        // Arrange
        var estoque = EstoqueTestDataFactory.CriarEstoquePadrao();

        // Act
        var acao = () => estoque.ObterItem(Guid.NewGuid());

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(EstoqueErrorMessages.ItemNaoEncontrado);
    }

}
