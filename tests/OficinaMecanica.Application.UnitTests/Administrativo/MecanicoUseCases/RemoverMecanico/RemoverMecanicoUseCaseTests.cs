using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.RemoverMecanico;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;

namespace OficinaMecanica.Application.UnitTests.Administrativo.MecanicoUseCases.RemoverMecanico;

public class RemoverMecanicoUseCaseTests
{
    [Fact]
    public async Task Dado_MecanicoExistente_Quando_RemoverMecanico_Entao_DeveRemoverMecanico()
    {
        // Arrange
        var mecanico = MecanicoTestDataFactory.CriarMecanicoPadrao();

        var repository = CriarRepository(mecanico);

        var useCase = CriarUseCase(repository);

        var request = MecanicoTestDataFactory.CriarRemoverMecanicoRequestValido(mecanico.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(mecanico.Id);

        repository.Verify(
            repo => repo.ObterPorIdAsync(
                request.MecanicoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            repo => repo.RemoverAsync(
                It.Is<Mecanico>(mecanicoRemovido => mecanicoRemovido.Id == mecanico.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_MecanicoInexistente_Quando_RemoverMecanico_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request = MecanicoTestDataFactory.CriarRemoverMecanicoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(MecanicoErrorMessages.MecanicoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(
            repo => repo.ObterPorIdAsync(
                request.MecanicoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            repo => repo.RemoverAsync(
                It.IsAny<Mecanico>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_RemoverMecanico_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IMecanicoRepository>();

        var useCase = CriarUseCase(repository);

        var request = MecanicoTestDataFactory.CriarRemoverMecanicoRequestValido(Guid.Empty);

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

        repository.Verify(
            repo => repo.RemoverAsync(
                It.IsAny<Mecanico>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Mock<IMecanicoRepository> CriarRepository(Mecanico? mecanico)
    {
        var repository = new Mock<IMecanicoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(mecanico);

        return repository;
    }

    private static RemoverMecanicoUseCase CriarUseCase(Mock<IMecanicoRepository> repository)
    {
        return new RemoverMecanicoUseCase(
            repository.Object,
            new RemoverMecanicoValidator(),
            MapperFactory.Criar());
    }
}