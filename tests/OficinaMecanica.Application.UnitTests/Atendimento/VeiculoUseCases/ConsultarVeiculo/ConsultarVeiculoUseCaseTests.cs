using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculo;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Atendimento.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.Messages;

namespace OficinaMecanica.Application.UnitTests.Atendimento.VeiculoUseCases.ConsultarVeiculo;

public class ConsultarVeiculoUseCaseTests
{
    [Fact]
    public async Task Dado_VeiculoExistente_Quando_ConsultarVeiculo_Entao_DeveRetornarDadosDoVeiculo()
    {
        // Arrange
        var veiculo = VeiculoTestDataFactory.CriarVeiculoPadrao();

        var repository = CriarRepository(veiculo);

        var useCase = CriarUseCase(repository);

        var request = VeiculoTestDataFactory.CriarConsultarVeiculoRequestValido(veiculo.Id);

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
            repo => repo.ObterPorIdAsync(
                request.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_VeiculoInexistente_Quando_ConsultarVeiculo_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request = VeiculoTestDataFactory.CriarConsultarVeiculoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(VeiculoErrorMessages.VeiculoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(
            repo => repo.ObterPorIdAsync(
                request.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_ConsultarVeiculo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IVeiculoRepository>();

        var useCase = CriarUseCase(repository);

        var request = VeiculoTestDataFactory.CriarConsultarVeiculoRequestValido(Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Mock<IVeiculoRepository> CriarRepository(Veiculo? veiculo)
    {
        var repository = new Mock<IVeiculoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        return repository;
    }

    private static ConsultarVeiculoUseCase CriarUseCase(Mock<IVeiculoRepository> repository)
    {
        return new ConsultarVeiculoUseCase(
            repository.Object,
            new ConsultarVeiculoValidator(),
            MapperFactory.Criar());
    }
}