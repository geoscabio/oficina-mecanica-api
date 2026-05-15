using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculoPorPlaca;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Atendimento.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.Messages;

namespace OficinaMecanica.Application.UnitTests.Atendimento.VeiculoUseCases.ConsultarVeiculoPorPlaca;

public class ConsultarVeiculoPorPlacaUseCaseTests
{
    [Fact]
    public async Task Dado_VeiculoExistente_Quando_ConsultarVeiculoPorPlaca_Entao_DeveRetornarDadosDoVeiculo()
    {
        // Arrange
        var veiculo = VeiculoTestDataFactory.CriarVeiculoPadrao();

        var repository = CriarRepository(veiculo);

        var useCase = CriarUseCase(repository);

        var request = VeiculoTestDataFactory.CriarConsultarVeiculoPorPlacaRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(veiculo.Id);
        resultado.Valor.ClienteId.Should().Be(veiculo.ClienteId);
        resultado.Valor.Placa.Should().Be(VeiculoTestDataFactory.PlacaNormalizadaPadrao);
        resultado.Valor.Marca.Should().Be(VeiculoTestDataFactory.MarcaPadrao);
        resultado.Valor.Modelo.Should().Be(VeiculoTestDataFactory.ModeloPadrao);
        resultado.Valor.Ano.Should().Be(VeiculoTestDataFactory.AnoPadrao);

        repository.Verify(
            repo => repo.ObterPorPlacaAsync(
                VeiculoTestDataFactory.PlacaNormalizadaPadrao,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_VeiculoInexistente_Quando_ConsultarVeiculoPorPlaca_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request = VeiculoTestDataFactory.CriarConsultarVeiculoPorPlacaRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(VeiculoErrorMessages.VeiculoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(
            repo => repo.ObterPorPlacaAsync(
                VeiculoTestDataFactory.PlacaNormalizadaPadrao,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_PlacaVazia_Quando_ConsultarVeiculoPorPlaca_Entao_DeveRetornarFalhaDeValidacao(string placa)
    {
        // Arrange
        var repository = new Mock<IVeiculoRepository>();

        var useCase = CriarUseCase(repository);

        var request = VeiculoTestDataFactory.CriarConsultarVeiculoPorPlacaRequestValido(
            placa);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.ObterPorPlacaAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Mock<IVeiculoRepository> CriarRepository(Veiculo? veiculo)
    {
        var repository = new Mock<IVeiculoRepository>();

        repository
            .Setup(repo => repo.ObterPorPlacaAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        return repository;
    }

    private static ConsultarVeiculoPorPlacaUseCase CriarUseCase(Mock<IVeiculoRepository> repository)
    {
        return new ConsultarVeiculoPorPlacaUseCase(
            repository.Object,
            new ConsultarVeiculoPorPlacaValidator(),
            MapperFactory.Criar());
    }
}