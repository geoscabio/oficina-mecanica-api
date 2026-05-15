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
    public void Dado_DadosValidos_Quando_CriarPecaInsumoCatalogo_Entao_DeveRegistrarItemComTipoEValor(TipoPecaInsumo tipo)
    {
        // Arrange
        const string descricaoEsperada = PecaInsumoCatalogoTestDataFactory.DescricaoPadrao;
        const decimal valorEsperado = PecaInsumoCatalogoTestDataFactory.ValorPadrao;

        // Act
        var item = PecaInsumoCatalogoTestDataFactory.CriarPecaInsumoCatalogoPadrao(tipo);

        // Assert
        item.Id.Should().NotBeEmpty();
        item.Descricao.Should().Be(descricaoEsperada);
        item.Tipo.Should().Be(tipo);
        item.Valor.Should().Be(valorEsperado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_DescricaoInvalida_Quando_CriarPecaInsumoCatalogo_Entao_DeveLancarDomainException(string descricao)
    {
        // Arrange
        const TipoPecaInsumo tipo = TipoPecaInsumo.PECA;
        const decimal valor = PecaInsumoCatalogoTestDataFactory.ValorPadrao;

        // Act
        var acao = () => PecaInsumoCatalogo.Criar(descricao, tipo, valor);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(PecaInsumoCatalogoErrorMessages.DescricaoObrigatoria);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Dado_ValorInvalido_Quando_CriarPecaInsumoCatalogo_Entao_DeveLancarDomainException(decimal valor)
    {
        // Arrange
        const string descricao = PecaInsumoCatalogoTestDataFactory.DescricaoPadrao;
        const TipoPecaInsumo tipo = TipoPecaInsumo.PECA;

        // Act
        var acao = () => PecaInsumoCatalogo.Criar(descricao, tipo, valor);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(PecaInsumoCatalogoErrorMessages.ValorMaiorQueZero);
    }

    [Fact]
    public void Dado_TipoInvalido_Quando_CriarPecaInsumoCatalogo_Entao_DeveLancarDomainException()
    {
        // Arrange
        const string descricao = PecaInsumoCatalogoTestDataFactory.DescricaoPadrao;
        const decimal valor = PecaInsumoCatalogoTestDataFactory.ValorPadrao;
        const TipoPecaInsumo tipoInvalido = (TipoPecaInsumo)99;

        // Act
        var acao = () => PecaInsumoCatalogo.Criar(descricao, tipoInvalido, valor);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(PecaInsumoCatalogoErrorMessages.TipoInvalido);
    }

    [Fact]
    public void Dado_DadosValidos_Quando_AtualizarPecaInsumoCatalogo_Entao_DeveAtualizarDados()
    {
        // Arrange
        var item = PecaInsumoCatalogo.Criar("Filtro de óleo", TipoPecaInsumo.PECA, 45m);

        // Act
        item.Atualizar("Óleo 5W30", TipoPecaInsumo.INSUMO, 38m);

        // Assert
        item.Descricao.Should().Be("Óleo 5W30");
        item.Tipo.Should().Be(TipoPecaInsumo.INSUMO);
        item.Valor.Should().Be(38m);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_DescricaoInvalida_Quando_AtualizarPecaInsumoCatalogo_Entao_DeveLancarDomainException(string descricao)
    {
        // Arrange
        var item = PecaInsumoCatalogo.Criar("Filtro de óleo", TipoPecaInsumo.PECA, 45m);

        // Act
        var acao = () => item.Atualizar(descricao, TipoPecaInsumo.PECA, 45m);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(PecaInsumoCatalogoErrorMessages.DescricaoObrigatoria);
    }

    [Fact]
    public void Dado_ValorInvalido_Quando_AtualizarPecaInsumoCatalogo_Entao_DeveLancarDomainException()
    {
        // Arrange
        var item = PecaInsumoCatalogo.Criar("Filtro de óleo", TipoPecaInsumo.PECA, 45m);

        // Act
        var acao = () => item.Atualizar("Filtro de óleo", TipoPecaInsumo.PECA, 0m);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(PecaInsumoCatalogoErrorMessages.ValorMaiorQueZero);
    }
}
