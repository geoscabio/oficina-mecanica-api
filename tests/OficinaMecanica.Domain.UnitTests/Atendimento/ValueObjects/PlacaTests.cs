using FluentAssertions;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Domain.Atendimento.ValueObjects;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.UnitTests.Atendimento.Factories;

namespace OficinaMecanica.Domain.UnitTests.Atendimento.ValueObjects;

public class PlacaTests
{
    [Theory]
    [InlineData("ABC-1234", VeiculoTestDataFactory.PlacaNormalizadaPadrao)]
    [InlineData("abc1d23", "ABC1D23")]
    public void Dado_PlacaValida_Quando_Criar_Entao_DeveNormalizarNumeroDaPlaca(
        string numero,
        string esperado)
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
    [InlineData("  ")]
    [InlineData("ABC123")]
    [InlineData("ABCD123")]
    [InlineData("ABC12D4")]
    public void Dado_PlacaInvalida_Quando_Criar_Entao_DeveLancarDomainException(string numero)
    {
        // Arrange
        var numeroInformado = numero;

        // Act
        var acao = () => Placa.Criar(numeroInformado);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(VeiculoErrorMessages.PlacaInvalida);
    }

    [Fact]
    public void Dado_PlacasComMesmosCaracteres_Quando_Comparar_Entao_DevemSerIguaisPorValor()
    {
        // Arrange
        var placaComMascara = Placa.Criar(VeiculoTestDataFactory.PlacaPadrao);
        var placaSemMascara = Placa.Criar(VeiculoTestDataFactory.PlacaNormalizadaPadrao.ToLowerInvariant());

        // Act
        var placasIguais = placaComMascara.Equals(placaSemMascara);

        // Assert
        placasIguais.Should().BeTrue();
        placaComMascara.Should().Be(placaSemMascara);
    }
}