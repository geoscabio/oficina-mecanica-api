using FluentAssertions;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Messages;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.UnitTests.Administrativo.Factories;

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
    public void Dado_NomeInvalido_Quando_CriarMecanico_Entao_DeveLancarDomainException(string nome)
    {
        // Arrange
        const string funcional = MecanicoTestDataFactory.FuncionalPadrao;

        // Act
        var acao = () => Mecanico.Criar(nome, funcional);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(MecanicoErrorMessages.NomeObrigatorio);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_FuncionalInvalido_Quando_CriarMecanico_Entao_DeveLancarDomainException(string funcional)
    {
        // Arrange
        const string nome = MecanicoTestDataFactory.NomePadrao;

        // Act
        var acao = () => Mecanico.Criar(nome, funcional);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(MecanicoErrorMessages.FuncionalObrigatorio);
    }

    [Fact]
    public void Dado_DadosValidos_Quando_AtualizarMecanico_Entao_DeveAtualizarDados()
    {
        // Arrange
        var mecanico = MecanicoTestDataFactory.CriarMecanicoPadrao();

        // Act
        mecanico.Atualizar("Carlos Silva", "MEC-002");

        // Assert
        mecanico.Nome.Should().Be("Carlos Silva");
        mecanico.Funcional.Should().Be("MEC-002");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_NomeInvalido_Quando_AtualizarMecanico_Entao_DeveLancarDomainException(string nome)
    {
        // Arrange
        var mecanico = MecanicoTestDataFactory.CriarMecanicoPadrao();

        // Act
        var acao = () => mecanico.Atualizar(nome, MecanicoTestDataFactory.FuncionalPadrao);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(MecanicoErrorMessages.NomeObrigatorio);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_FuncionalInvalido_Quando_AtualizarMecanico_Entao_DeveLancarDomainException(string funcional)
    {
        // Arrange
        var mecanico = MecanicoTestDataFactory.CriarMecanicoPadrao();

        // Act
        var acao = () => mecanico.Atualizar(MecanicoTestDataFactory.NomePadrao, funcional);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(MecanicoErrorMessages.FuncionalObrigatorio);
    }
}
