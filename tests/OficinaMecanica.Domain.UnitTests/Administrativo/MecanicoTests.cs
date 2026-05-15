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
        const string nome = MecanicoTestDataFactory.NomePadrao;
        const string funcional = MecanicoTestDataFactory.FuncionalPadrao;

        // Act
        var mecanico = Mecanico.Criar(
            nome,
            funcional);

        // Assert
        mecanico.Id.Should().NotBeEmpty();
        mecanico.Nome.Should().Be(MecanicoTestDataFactory.NomePadrao);
        mecanico.Funcional.Should().Be(MecanicoTestDataFactory.FuncionalPadrao);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_NomeInvalido_Quando_CriarMecanico_Entao_DeveLancarDomainException(string nome)
    {
        // Arrange
        const string funcional = MecanicoTestDataFactory.FuncionalPadrao;

        // Act
        var acao = () => Mecanico.Criar(
            nome,
            funcional);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(MecanicoErrorMessages.NomeObrigatorio);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_FuncionalInvalido_Quando_CriarMecanico_Entao_DeveLancarDomainException(
        string funcional)
    {
        // Arrange
        const string nome = MecanicoTestDataFactory.NomePadrao;

        // Act
        var acao = () => Mecanico.Criar(
            nome,
            funcional);

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
        mecanico.Atualizar(
            MecanicoTestDataFactory.NomeAtualizado,
            MecanicoTestDataFactory.FuncionalAtualizado);

        // Assert
        mecanico.Nome.Should().Be(MecanicoTestDataFactory.NomeAtualizado);
        mecanico.Funcional.Should().Be(MecanicoTestDataFactory.FuncionalAtualizado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_NomeInvalido_Quando_AtualizarMecanico_Entao_DeveLancarDomainException(
        string nome)
    {
        // Arrange
        var mecanico = MecanicoTestDataFactory.CriarMecanicoPadrao();

        // Act
        var acao = () => mecanico.Atualizar(
            nome,
            MecanicoTestDataFactory.FuncionalAtualizado);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(MecanicoErrorMessages.NomeObrigatorio);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_FuncionalInvalido_Quando_AtualizarMecanico_Entao_DeveLancarDomainException(
        string funcional)
    {
        // Arrange
        var mecanico = MecanicoTestDataFactory.CriarMecanicoPadrao();

        // Act
        var acao = () => mecanico.Atualizar(
            MecanicoTestDataFactory.NomeAtualizado,
            funcional);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(MecanicoErrorMessages.FuncionalObrigatorio);
    }
}