using FluentAssertions;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Enums;
using OficinaMecanica.Domain.Administrativo.Messages;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.UnitTests.Administrativo.Factories;

namespace OficinaMecanica.Domain.UnitTests.Administrativo;

public class PecaInsumoCatalogoTests
{
    [Theory]
    [InlineData(TipoPecaInsumo.PECA)]
    [InlineData(TipoPecaInsumo.INSUMO)]
    public void Dado_DadosValidos_Quando_CriarPecaInsumoCatalogo_Entao_DeveRegistrarItemComTipoEValor(
        TipoPecaInsumo tipo)
    {
        // Arrange
        const string descricao = PecaInsumoCatalogoTestDataFactory.DescricaoPadrao;
        const decimal valor = PecaInsumoCatalogoTestDataFactory.ValorPadrao;

        // Act
        var item = PecaInsumoCatalogo.Criar(
            descricao,
            tipo,
            valor);

        // Assert
        item.Id.Should().NotBeEmpty();
        item.Descricao.Should().Be(PecaInsumoCatalogoTestDataFactory.DescricaoPadrao);
        item.Tipo.Should().Be(tipo);
        item.Valor.Should().Be(PecaInsumoCatalogoTestDataFactory.ValorPadrao);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_DescricaoInvalida_Quando_CriarPecaInsumoCatalogo_Entao_DeveLancarDomainException(
        string descricao)
    {
        // Arrange
        const TipoPecaInsumo tipo = PecaInsumoCatalogoTestDataFactory.TipoPadrao;
        const decimal valor = PecaInsumoCatalogoTestDataFactory.ValorPadrao;

        // Act
        var acao = () => PecaInsumoCatalogo.Criar(
            descricao,
            tipo,
            valor);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(PecaInsumoCatalogoErrorMessages.DescricaoObrigatoria);
    }

    [Fact]
    public void Dado_TipoInvalido_Quando_CriarPecaInsumoCatalogo_Entao_DeveLancarDomainException()
    {
        // Arrange
        const string descricao = PecaInsumoCatalogoTestDataFactory.DescricaoPadrao;
        const TipoPecaInsumo tipo = (TipoPecaInsumo)99;
        const decimal valor = PecaInsumoCatalogoTestDataFactory.ValorPadrao;

        // Act
        var acao = () => PecaInsumoCatalogo.Criar(
            descricao,
            tipo,
            valor);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(PecaInsumoCatalogoErrorMessages.TipoInvalido);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Dado_ValorInvalido_Quando_CriarPecaInsumoCatalogo_Entao_DeveLancarDomainException(
        decimal valor)
    {
        // Arrange
        const string descricao = PecaInsumoCatalogoTestDataFactory.DescricaoPadrao;
        const TipoPecaInsumo tipo = PecaInsumoCatalogoTestDataFactory.TipoPadrao;

        // Act
        var acao = () => PecaInsumoCatalogo.Criar(
            descricao,
            tipo,
            valor);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(PecaInsumoCatalogoErrorMessages.ValorMaiorQueZero);
    }

    [Fact]
    public void Dado_DadosValidos_Quando_AtualizarPecaInsumoCatalogo_Entao_DeveAtualizarDados()
    {
        // Arrange
        var item = PecaInsumoCatalogoTestDataFactory.CriarPecaInsumoCatalogoPadrao();

        // Act
        item.Atualizar(
            PecaInsumoCatalogoTestDataFactory.DescricaoAtualizada,
            PecaInsumoCatalogoTestDataFactory.TipoAtualizado,
            PecaInsumoCatalogoTestDataFactory.ValorAtualizado);

        // Assert
        item.Descricao.Should().Be(PecaInsumoCatalogoTestDataFactory.DescricaoAtualizada);
        item.Tipo.Should().Be(PecaInsumoCatalogoTestDataFactory.TipoAtualizado);
        item.Valor.Should().Be(PecaInsumoCatalogoTestDataFactory.ValorAtualizado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_DescricaoInvalida_Quando_AtualizarPecaInsumoCatalogo_Entao_DeveLancarDomainException(
        string descricao)
    {
        // Arrange
        var item = PecaInsumoCatalogoTestDataFactory.CriarPecaInsumoCatalogoPadrao();

        // Act
        var acao = () => item.Atualizar(
            descricao,
            PecaInsumoCatalogoTestDataFactory.TipoAtualizado,
            PecaInsumoCatalogoTestDataFactory.ValorAtualizado);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(PecaInsumoCatalogoErrorMessages.DescricaoObrigatoria);
    }

    [Fact]
    public void Dado_TipoInvalido_Quando_AtualizarPecaInsumoCatalogo_Entao_DeveLancarDomainException()
    {
        // Arrange
        var item = PecaInsumoCatalogoTestDataFactory.CriarPecaInsumoCatalogoPadrao();

        // Act
        var acao = () => item.Atualizar(
            PecaInsumoCatalogoTestDataFactory.DescricaoAtualizada,
            (TipoPecaInsumo)99,
            PecaInsumoCatalogoTestDataFactory.ValorAtualizado);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(PecaInsumoCatalogoErrorMessages.TipoInvalido);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Dado_ValorInvalido_Quando_AtualizarPecaInsumoCatalogo_Entao_DeveLancarDomainException(
        decimal valor)
    {
        // Arrange
        var item = PecaInsumoCatalogoTestDataFactory.CriarPecaInsumoCatalogoPadrao();

        // Act
        var acao = () => item.Atualizar(
            PecaInsumoCatalogoTestDataFactory.DescricaoAtualizada,
            PecaInsumoCatalogoTestDataFactory.TipoAtualizado,
            valor);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(PecaInsumoCatalogoErrorMessages.ValorMaiorQueZero);
    }
}