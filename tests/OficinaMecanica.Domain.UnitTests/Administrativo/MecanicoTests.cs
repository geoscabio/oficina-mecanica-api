using FluentAssertions;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Exceptions;
using OficinaMecanica.Domain.UnitTests.Administrativo.Builders;

namespace OficinaMecanica.Domain.UnitTests.Administrativo;

public class MecanicoTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_CriarMecanico_Entao_DeveRegistrarMecanicoComIdentidade()
    {
        // Arrange
        const string nomeEsperado = MecanicoTestDataFactory.NomePadrao;
        const string funcionalEsperado = MecanicoTestDataFactory.FuncionalPadrao;

        // Act
        var mecanico = MecanicoTestDataFactory.CriarMecanicoPadrao();

        // Assert
        mecanico.Id.Should().NotBeEmpty();
        mecanico.Nome.Should().Be(nomeEsperado);
        mecanico.Funcional.Should().Be(funcionalEsperado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_NomeInvalido_Quando_CriarMecanico_Entao_DeveLancarMecanicoInvalidoException(string nome)
    {
        // Arrange
        const string funcional = MecanicoTestDataFactory.FuncionalPadrao;

        // Act
        var acao = () => Mecanico.Criar(nome, funcional);

        // Assert
        acao.Should().Throw<MecanicoInvalidoException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_FuncionalInvalido_Quando_CriarMecanico_Entao_DeveLancarMecanicoInvalidoException(string funcional)
    {
        // Arrange
        const string nome = MecanicoTestDataFactory.NomePadrao;

        // Act
        var acao = () => Mecanico.Criar(nome, funcional);

        // Assert
        acao.Should().Throw<MecanicoInvalidoException>();
    }
}
