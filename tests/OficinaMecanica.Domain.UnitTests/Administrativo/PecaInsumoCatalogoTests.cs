using FluentAssertions;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Enums;
using OficinaMecanica.Domain.Administrativo.Exceptions;

namespace OficinaMecanica.Domain.UnitTests.Administrativo;

public class PecaInsumoCatalogoTests
{
    [Theory]
    [InlineData(TipoPecaInsumo.PECA)]
    [InlineData(TipoPecaInsumo.INSUMO)]
    public void Dado_DadosValidos_Quando_CriarPecaInsumoCatalogo_Entao_DeveRegistrarItemComTipoEValor(TipoPecaInsumo tipo)
    {
        var item = PecaInsumoCatalogo.Criar("Filtro de oleo", tipo, 45m);

        item.Id.Should().NotBeEmpty();
        item.Descricao.Should().Be("Filtro de oleo");
        item.Tipo.Should().Be(tipo);
        item.Valor.Should().Be(45m);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_DescricaoInvalida_Quando_CriarPecaInsumoCatalogo_Entao_DeveLancarPecaInsumoCatalogoInvalidaException(string descricao)
    {
        var acao = () => PecaInsumoCatalogo.Criar(descricao, TipoPecaInsumo.PECA, 45m);

        acao.Should().Throw<PecaInsumoCatalogoInvalidaException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Dado_ValorInvalido_Quando_CriarPecaInsumoCatalogo_Entao_DeveLancarPecaInsumoCatalogoInvalidaException(decimal valor)
    {
        var acao = () => PecaInsumoCatalogo.Criar("Filtro de oleo", TipoPecaInsumo.PECA, valor);

        acao.Should().Throw<PecaInsumoCatalogoInvalidaException>();
    }

    [Fact]
    public void Dado_TipoInvalido_Quando_CriarPecaInsumoCatalogo_Entao_DeveLancarPecaInsumoCatalogoInvalidaException()
    {
        var acao = () => PecaInsumoCatalogo.Criar("Filtro de oleo", (TipoPecaInsumo)99, 45m);

        acao.Should().Throw<PecaInsumoCatalogoInvalidaException>();
    }
}
