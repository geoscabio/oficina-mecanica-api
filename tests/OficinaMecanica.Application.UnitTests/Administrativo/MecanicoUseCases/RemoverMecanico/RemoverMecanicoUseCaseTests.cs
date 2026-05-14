using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.RemoverMecanico;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Builders;
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
        var repository = new Mock<IMecanicoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(mecanico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(mecanico);

        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new RemoverMecanicoRequest(mecanico.Id));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(mecanico.Id);

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
        var repository = new Mock<IMecanicoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new RemoverMecanicoRequest(Guid.NewGuid()));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(MecanicoErrorMessages.MecanicoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(
            repo => repo.RemoverAsync(It.IsAny<Mecanico>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_RemoverMecanico_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IMecanicoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new RemoverMecanicoRequest(Guid.Empty));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static RemoverMecanicoUseCase CriarUseCase(Mock<IMecanicoRepository> repository)
    {
        return new RemoverMecanicoUseCase(
            repository.Object,
            new RemoverMecanicoValidator(),
            MapperFactory.Criar());
    }
}