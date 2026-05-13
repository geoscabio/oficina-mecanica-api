using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculoPorPlaca;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.Atendimento.Builders;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;

namespace OficinaMecanica.Application.UnitTests.Atendimento.VeiculoUseCases.ConsultarVeiculoPorPlaca;

public class ConsultarVeiculoPorPlacaUseCaseTests
{
    [Fact]
    public async Task Dado_VeiculoExistente_Quando_ConsultarVeiculoPorPlaca_Entao_DeveRetornarDadosDoVeiculo()
    {
        // Arrange
        var veiculo = VeiculoTestDataFactory.CriarVeiculoPadrao();
        var repository = new Mock<IVeiculoRepository>();
        repository
            .Setup(repo => repo.ObterPorPlacaAsync("ABC1234", It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarVeiculoPorPlacaRequest("ABC-1234"));

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
    public async Task Dado_VeiculoInexistente_Quando_ConsultarVeiculoPorPlaca_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = new Mock<IVeiculoRepository>();
        repository
            .Setup(repo => repo.ObterPorPlacaAsync("ABC1234", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Veiculo?)null);

        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarVeiculoPorPlacaRequest("ABC-1234"));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(VeiculoErrorMessages.VeiculoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_PlacaVazia_Quando_ConsultarVeiculoPorPlaca_Entao_DeveRetornarFalhaDeValidacao(string placa)
    {
        // Arrange
        var repository = new Mock<IVeiculoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarVeiculoPorPlacaRequest(placa));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static ConsultarVeiculoPorPlacaUseCase CriarUseCase(Mock<IVeiculoRepository> repository)
    {
        return new ConsultarVeiculoPorPlacaUseCase(
            repository.Object,
            new ConsultarVeiculoPorPlacaValidator(),
            MapperFactory.Criar());
    }
}







