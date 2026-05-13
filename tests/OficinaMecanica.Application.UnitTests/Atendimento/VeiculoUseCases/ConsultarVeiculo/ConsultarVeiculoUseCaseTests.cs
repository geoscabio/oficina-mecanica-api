using FluentAssertions;
using FluentValidation;
using Moq;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculo;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Atendimento.Interfaces;

namespace OficinaMecanica.Application.UnitTests.Atendimento.VeiculoUseCases.ConsultarVeiculo;

public class ConsultarVeiculoUseCaseTests
{
    [Fact]
    public async Task Dado_VeiculoExistente_Quando_ConsultarVeiculo_Entao_DeveRetornarDadosDoVeiculo()
    {
        // Arrange
        var veiculo = TestDataFactory.CriarVeiculoPadrao();
        var repository = new Mock<IVeiculoRepository>();
        repository
            .Setup(repo => repo.ObterPorIdAsync(veiculo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarVeiculoRequest(veiculo.Id));

        // Assert
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
    public async Task Dado_VeiculoInexistente_Quando_ConsultarVeiculo_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = new Mock<IVeiculoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarVeiculoRequest(Guid.NewGuid()));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Be("Veiculo nao encontrado.");
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_ConsultarVeiculo_Entao_DeveLancarValidationException()
    {
        // Arrange
        var repository = new Mock<IVeiculoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var acao = () => useCase.ExecuteAsync(new ConsultarVeiculoRequest(Guid.Empty));

        // Assert
        await acao.Should().ThrowAsync<ValidationException>();
    }

    private static ConsultarVeiculoUseCase CriarUseCase(Mock<IVeiculoRepository> repository)
    {
        return new ConsultarVeiculoUseCase(
            repository.Object,
            new ConsultarVeiculoValidator(),
            MapperFactory.Criar());
    }
}
