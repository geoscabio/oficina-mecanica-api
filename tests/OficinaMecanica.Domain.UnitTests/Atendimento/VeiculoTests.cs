using FluentAssertions;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Exceptions;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Domain.UnitTests.Atendimento;

public class VeiculoTests
{
    [Fact]
    public void Dado_DadosValidos_Quando_CriarVeiculo_Entao_DeveRegistrarVeiculoVinculadoAoCliente()
    {
        var clienteId = Guid.NewGuid();
        var placa = Placa.Criar("ABC-1234");

        var veiculo = Veiculo.Criar(clienteId, placa, "Toyota", "Corolla", 2020);

        veiculo.Id.Should().NotBeEmpty();
        veiculo.ClienteId.Should().Be(clienteId);
        veiculo.Placa.Should().Be(placa);
        veiculo.Marca.Should().Be("Toyota");
        veiculo.Modelo.Should().Be("Corolla");
        veiculo.Ano.Should().Be(2020);
    }

    [Fact]
    public void Dado_ClienteIdVazio_Quando_CriarVeiculo_Entao_DeveLancarVeiculoInvalidoException()
    {
        var placa = Placa.Criar("ABC-1234");

        var acao = () => Veiculo.Criar(Guid.Empty, placa, "Toyota", "Corolla", 2020);

        acao.Should().Throw<VeiculoInvalidoException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public void Dado_MarcaInvalida_Quando_CriarVeiculo_Entao_DeveLancarVeiculoInvalidoException(string marca)
    {
        var placa = Placa.Criar("ABC-1234");

        var acao = () => Veiculo.Criar(Guid.NewGuid(), placa, marca, "Corolla", 2020);

        acao.Should().Throw<VeiculoInvalidoException>();
    }
}
