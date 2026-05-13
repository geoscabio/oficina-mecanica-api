using FluentAssertions;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Exceptions;
using OficinaMecanica.Domain.UnitTests.Administrativo.Builders;

namespace OficinaMecanica.Domain.UnitTests.Administrativo;

public class ServicoCatalogoTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_CriarServicoCatalogo_Entao_DeveRegistrarServicoComValor()
    {
        // Arrange
        const string descricaoEsperada = ServicoCatalogoTestDataFactory.DescricaoPadrao;
        const decimal valorEsperado = ServicoCatalogoTestDataFactory.ValorPadrao;

        // Act
        var servico = ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao();

        // Assert
        servico.Id.Should().NotBeEmpty();
        servico.Descricao.Should().Be(descricaoEsperada);
        servico.Valor.Should().Be(valorEsperado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_DescricaoInvalida_Quando_CriarServicoCatalogo_Entao_DeveLancarServicoCatalogoInvalidoException(string descricao)
    {
        // Arrange
        const decimal valor = ServicoCatalogoTestDataFactory.ValorPadrao;

        // Act
        var acao = () => ServicoCatalogo.Criar(descricao, valor);

        // Assert
        acao.Should().Throw<ServicoCatalogoInvalidoException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Dado_ValorInvalido_Quando_CriarServicoCatalogo_Entao_DeveLancarServicoCatalogoInvalidoException(decimal valor)
    {
        // Arrange
        const string descricao = ServicoCatalogoTestDataFactory.DescricaoPadrao;

        // Act
        var acao = () => ServicoCatalogo.Criar(descricao, valor);

        // Assert
        acao.Should().Throw<ServicoCatalogoInvalidoException>();
    }
}
