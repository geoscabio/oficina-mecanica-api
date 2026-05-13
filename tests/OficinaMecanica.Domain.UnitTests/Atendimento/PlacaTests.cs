using FluentAssertions;
using OficinaMecanica.Domain.Atendimento.Exceptions;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Domain.UnitTests.Atendimento;

public class PlacaTests
{
    [Theory]
    [InlineData("ABC-1234", "ABC1234")]
    [InlineData("abc1d23", "ABC1D23")]
    public void Dado_PlacaValida_Quando_Criar_Entao_DeveNormalizarNumeroDaPlaca(string numero, string esperado)
    {
        // Arrange
        var numeroInformado = numero;

        // Act
        var placa = Placa.Criar(numeroInformado);

        // Assert
        placa.NumeroPlaca.Should().Be(esperado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ABC123")]
    [InlineData("ABCD123")]
    [InlineData("ABC12D4")]
    public void Dado_PlacaInvalida_Quando_Criar_Entao_DeveLancarPlacaInvalidaException(string numero)
    {
        // Arrange
        var numeroInformado = numero;

        // Act
        var acao = () => Placa.Criar(numeroInformado);

        // Assert
        acao.Should().Throw<PlacaInvalidaException>();
    }

    [Fact]
    public void Dado_PlacasComMesmosCaracteres_Quando_Comparar_Entao_DevemSerIguaisPorValor()
    {
        // Arrange
        var placaComMascara = Placa.Criar("ABC-1234");
        var placaSemMascara = Placa.Criar("abc1234");

        // Act
        var placasIguais = placaComMascara.Equals(placaSemMascara);

        // Assert
        placasIguais.Should().BeTrue();
        placaComMascara.Should().Be(placaSemMascara);
    }
}
