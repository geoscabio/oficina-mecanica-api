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

        var repository = CriarRepository(servicoCatalogo);

        var useCase = CriarUseCase(repository);

        var request = ServicoCatalogoTestDataFactory.CriarRemoverServicoCatalogoRequestValido(servicoCatalogo.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(servicoCatalogo.Id);

        repository.Verify(repo => repo.ObterPorIdAsync(request.ServicoCatalogoId, It.IsAny<CancellationToken>()), Times.Once);

        repository.Verify(repo => repo.RemoverAsync(It.Is<ServicoCatalogo>(servico => servico.Id == servicoCatalogo.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_ServicoCatalogoInexistente_Quando_RemoverServicoCatalogo_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request = ServicoCatalogoTestDataFactory.CriarRemoverServicoCatalogoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(ServicoCatalogoErrorMessages.ServicoCatalogoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(repo => repo.ObterPorIdAsync(request.ServicoCatalogoId, It.IsAny<CancellationToken>()), Times.Once);

        repository.Verify(repo => repo.RemoverAsync(It.IsAny<ServicoCatalogo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_RemoverServicoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IServicoCatalogoRepository>();

        var useCase = CriarUseCase(repository);

        var request = ServicoCatalogoTestDataFactory.CriarRemoverServicoCatalogoRequestValido(Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.RemoverAsync(It.IsAny<ServicoCatalogo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IServicoCatalogoRepository> CriarRepository(ServicoCatalogo? servicoCatalogo)
    {
        var repository = new Mock<IServicoCatalogoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(servicoCatalogo);

        return repository;
    }

    private static RemoverServicoCatalogoUseCase CriarUseCase(Mock<IServicoCatalogoRepository> repository)
    {
        return new RemoverServicoCatalogoUseCase(repository.Object, new RemoverServicoCatalogoValidator(), MapperFactory.Criar());
    }
}