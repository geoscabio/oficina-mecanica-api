using FluentAssertions;
using FluentValidation;
using Moq;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculoPorPlaca;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Application.UnitTests.Atendimento.VeiculoUseCases.ConsultarVeiculoPorPlaca;

public class ConsultarVeiculoPorPlacaUseCaseTests
{
    [Fact]
    public async Task Dado_VeiculoExistente_Quando_ConsultarVeiculoPorPlaca_Entao_DeveRetornarDadosDoVeiculo()
    {
        var veiculo = CriarVeiculo();
        var repository = new Mock<IVeiculoRepository>();
        repository
            .Setup(repo => repo.ObterPorPlacaAsync("ABC1234", It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);
        var useCase = new ConsultarVeiculoPorPlacaUseCase(
            repository.Object,
            new ConsultarVeiculoPorPlacaValidator(),
            MapperFactory.Criar());

        var resultado = await useCase.ExecuteAsync(new ConsultarVeiculoPorPlacaRequest("ABC-1234"));

        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(veiculo.Id);
        resultado.Valor.ClienteId.Should().Be(veiculo.ClienteId);
        resultado.Valor.Placa.Should().Be("ABC1234");
        resultado.Valor.Marca.Should().Be("Toyota");
        resultado.Valor.Modelo.Should().Be("Corolla");
        resultado.Valor.Ano.Should().Be(2020);
    }

    [Fact]
    public async Task Dado_VeiculoInexistente_Quando_ConsultarVeiculoPorPlaca_Entao_DeveRetornarFalha()
    {
        var repository = new Mock<IVeiculoRepository>();
        repository
            .Setup(repo => repo.ObterPorPlacaAsync("ABC1234", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Veiculo?)null);
        var useCase = new ConsultarVeiculoPorPlacaUseCase(
            repository.Object,
            new ConsultarVeiculoPorPlacaValidator(),
            MapperFactory.Criar());

        var resultado = await useCase.ExecuteAsync(new ConsultarVeiculoPorPlacaRequest("ABC-1234"));

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Be("Veiculo nao encontrado.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_PlacaVazia_Quando_ConsultarVeiculoPorPlaca_Entao_DeveLancarValidationException(string placa)
    {
        var repository = new Mock<IVeiculoRepository>();
        var useCase = new ConsultarVeiculoPorPlacaUseCase(
            repository.Object,
            new ConsultarVeiculoPorPlacaValidator(),
            MapperFactory.Criar());

        var acao = () => useCase.ExecuteAsync(new ConsultarVeiculoPorPlacaRequest(placa));

        await acao.Should().ThrowAsync<ValidationException>();
    }

    private static Veiculo CriarVeiculo()
    {
        return Veiculo.Criar(
            Guid.NewGuid(),
            Placa.Criar("ABC-1234"),
            "Toyota",
            "Corolla",
            2020);
    }
}
