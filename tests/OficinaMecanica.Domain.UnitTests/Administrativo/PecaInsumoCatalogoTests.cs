using FluentAssertions;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Enums;
using OficinaMecanica.Domain.Administrativo.Exceptions;
using OficinaMecanica.Domain.UnitTests.Administrativo.Builders;

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
    public void Dado_DescricaoInvalida_Quando_CriarPecaInsumoCatalogo_Entao_DeveLancarPecaInsumoCatalogoInvalidaException(string descricao)
    {
        // Arrange
        const TipoPecaInsumo tipo = TipoPecaInsumo.PECA;
        const decimal valor = PecaInsumoCatalogoTestDataFactory.ValorPadrao;

        // Act
        var acao = () => PecaInsumoCatalogo.Criar(descricao, tipo, valor);

        // Assert
        acao.Should().Throw<PecaInsumoCatalogoInvalidaException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Dado_ValorInvalido_Quando_CriarPecaInsumoCatalogo_Entao_DeveLancarPecaInsumoCatalogoInvalidaException(decimal valor)
    {
        // Arrange
        const string descricao = PecaInsumoCatalogoTestDataFactory.DescricaoPadrao;
        const TipoPecaInsumo tipo = TipoPecaInsumo.PECA;

        // Act
        var acao = () => PecaInsumoCatalogo.Criar(descricao, tipo, valor);

        // Assert
        acao.Should().Throw<PecaInsumoCatalogoInvalidaException>();
    }

    [Fact]
    public void Dado_TipoInvalido_Quando_CriarPecaInsumoCatalogo_Entao_DeveLancarPecaInsumoCatalogoInvalidaException()
    {
        // Arrange
        const string descricao = PecaInsumoCatalogoTestDataFactory.DescricaoPadrao;
        const decimal valor = PecaInsumoCatalogoTestDataFactory.ValorPadrao;
        const TipoPecaInsumo tipoInvalido = (TipoPecaInsumo)99;

        // Act
        var acao = () => PecaInsumoCatalogo.Criar(descricao, tipoInvalido, valor);

        // Assert
        acao.Should().Throw<PecaInsumoCatalogoInvalidaException>();
    }
}
