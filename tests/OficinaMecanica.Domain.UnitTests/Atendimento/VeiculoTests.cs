using FluentAssertions;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.UnitTests.Atendimento.Builders;

namespace OficinaMecanica.Domain.UnitTests.Atendimento;

public class VeiculoTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_CriarVeiculo_Entao_DeveRegistrarVeiculoVinculadoAoCliente()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var placa = VeiculoTestDataFactory.CriarPlacaPadrao();

        // Act
        var veiculo = Veiculo.Criar(
            clienteId,
            placa,
            VeiculoTestDataFactory.MarcaPadrao,
            VeiculoTestDataFactory.ModeloPadrao,
            VeiculoTestDataFactory.AnoPadrao);

        // Assert
        veiculo.Id.Should().NotBeEmpty();
        veiculo.ClienteId.Should().Be(clienteId);
        veiculo.Placa.Should().Be(placa);
        veiculo.Marca.Should().Be(VeiculoTestDataFactory.MarcaPadrao);
        veiculo.Modelo.Should().Be(VeiculoTestDataFactory.ModeloPadrao);
        veiculo.Ano.Should().Be(VeiculoTestDataFactory.AnoPadrao);
    }

    [Fact]
    public void Dado_ClienteIdVazio_Quando_CriarVeiculo_Entao_DeveLancarDomainException()
    {
        // Arrange
        var placa = VeiculoTestDataFactory.CriarPlacaPadrao();

        // Act
        var acao = () => Veiculo.Criar(
            Guid.Empty,
            placa,
            VeiculoTestDataFactory.MarcaPadrao,
            VeiculoTestDataFactory.ModeloPadrao,
            VeiculoTestDataFactory.AnoPadrao);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(VeiculoErrorMessages.ClienteObrigatorio);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_MarcaInvalida_Quando_CriarVeiculo_Entao_DeveLancarDomainException(string marca)
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var placa = VeiculoTestDataFactory.CriarPlacaPadrao();

        // Act
        var acao = () => Veiculo.Criar(
            clienteId,
            placa,
            marca,
            VeiculoTestDataFactory.ModeloPadrao,
            VeiculoTestDataFactory.AnoPadrao);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(VeiculoErrorMessages.MarcaObrigatoria);
    }

    [Fact]
    public void Dado_DadosValidos_Quando_AtualizarVeiculo_Entao_DeveAtualizarDados()
    {
        // Arrange
        var veiculo = VeiculoTestDataFactory.CriarVeiculoPadrao();
        var novaPlaca = VeiculoTestDataFactory.CriarPlacaPadrao();

        // Act
        veiculo.Atualizar(novaPlaca, "Honda", "Civic", 2022);

        // Assert
        veiculo.Placa.Should().Be(novaPlaca);
        veiculo.Marca.Should().Be("Honda");
        veiculo.Modelo.Should().Be("Civic");
        veiculo.Ano.Should().Be(2022);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_MarcaInvalida_Quando_AtualizarVeiculo_Entao_DeveLancarDomainException(string marca)
    {
        // Arrange
        var veiculo = VeiculoTestDataFactory.CriarVeiculoPadrao();

        // Act
        var acao = () => veiculo.Atualizar(
            VeiculoTestDataFactory.CriarPlacaPadrao(),
            marca,
            "Civic",
            2022);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(VeiculoErrorMessages.MarcaObrigatoria);
    }

    [Fact]
    public void Dado_AnoInvalido_Quando_AtualizarVeiculo_Entao_DeveLancarDomainException()
    {
        // Arrange
        var veiculo = VeiculoTestDataFactory.CriarVeiculoPadrao();

        // Act
        var acao = () => veiculo.Atualizar(
            VeiculoTestDataFactory.CriarPlacaPadrao(),
            "Honda",
            "Civic",
            0);

        // Assert
        acao.Should()
            .Throw<DomainException>()
            .WithMessage(VeiculoErrorMessages.AnoInvalido);
    }
}
