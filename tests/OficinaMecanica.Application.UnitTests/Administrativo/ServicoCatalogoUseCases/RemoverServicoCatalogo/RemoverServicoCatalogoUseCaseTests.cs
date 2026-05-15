using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.RemoverServicoCatalogo;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;

namespace OficinaMecanica.Application.UnitTests.Administrativo.ServicoCatalogoUseCases.RemoverServicoCatalogo;

public class RemoverServicoCatalogoUseCaseTests
{
    [Fact]
    public async Task Dado_ServicoCatalogoExistente_Quando_RemoverServicoCatalogo_Entao_DeveRemoverServicoCatalogo()
    {
        // Arrange
        var servicoCatalogo = ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao();
        var repository = new Mock<IServicoCatalogoRepository>();
        repository
            .Setup(repo => repo.ObterPorIdAsync(servicoCatalogo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(servicoCatalogo);

        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new RemoverServicoCatalogoRequest(servicoCatalogo.Id));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(servicoCatalogo.Id);
        repository.Verify(
            repo => repo.RemoverAsync(
                It.Is<ServicoCatalogo>(servico => servico.Id == servicoCatalogo.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_ServicoCatalogoInexistente_Quando_RemoverServicoCatalogo_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = new Mock<IServicoCatalogoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new RemoverServicoCatalogoRequest(Guid.NewGuid()));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(ServicoCatalogoErrorMessages.ServicoCatalogoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);
        repository.Verify(
            repo => repo.RemoverAsync(It.IsAny<ServicoCatalogo>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_RemoverServicoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IServicoCatalogoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new RemoverServicoCatalogoRequest(Guid.Empty));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static RemoverServicoCatalogoUseCase CriarUseCase(Mock<IServicoCatalogoRepository> repository)
    {
        return new RemoverServicoCatalogoUseCase(
            repository.Object,
            new RemoverServicoCatalogoValidator(),
            MapperFactory.Criar());
    }
}
