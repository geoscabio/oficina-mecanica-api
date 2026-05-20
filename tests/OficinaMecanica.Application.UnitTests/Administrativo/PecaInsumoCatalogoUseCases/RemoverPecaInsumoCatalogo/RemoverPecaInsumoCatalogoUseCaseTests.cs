using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.RemoverPecaInsumoCatalogo;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;

namespace OficinaMecanica.Application.UnitTests.Administrativo.PecaInsumoCatalogoUseCases.RemoverPecaInsumoCatalogo;

public class RemoverPecaInsumoCatalogoUseCaseTests
{
    [Fact]
    public async Task Dado_PecaInsumoCatalogoExistente_Quando_RemoverPecaInsumoCatalogo_Entao_DeveRemoverPecaInsumoCatalogo()
    {
        // Arrange
        var item = PecaInsumoCatalogoTestDataFactory.CriarPecaInsumoCatalogoPadrao();

        var repository = CriarRepository(item);

        var useCase = CriarUseCase(repository);

        var request = PecaInsumoCatalogoTestDataFactory.CriarRemoverPecaInsumoCatalogoRequestValido(item.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(item.Id);

        repository.Verify(repo => repo.ObterPorIdAsync(request.PecaInsumoCatalogoId, It.IsAny<CancellationToken>()), Times.Once);

        repository.Verify(repo => repo.RemoverAsync(It.Is<PecaInsumoCatalogo>(itemRemovido => itemRemovido.Id == item.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_PecaInsumoCatalogoInexistente_Quando_RemoverPecaInsumoCatalogo_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request = PecaInsumoCatalogoTestDataFactory.CriarRemoverPecaInsumoCatalogoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(PecaInsumoCatalogoErrorMessages.PecaInsumoCatalogoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(repo => repo.ObterPorIdAsync(request.PecaInsumoCatalogoId, It.IsAny<CancellationToken>()), Times.Once);

        repository.Verify(repo => repo.RemoverAsync(It.IsAny<PecaInsumoCatalogo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_RemoverPecaInsumoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IPecaInsumoCatalogoRepository>();

        var useCase = CriarUseCase(repository);

        var request = PecaInsumoCatalogoTestDataFactory.CriarRemoverPecaInsumoCatalogoRequestValido(Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.RemoverAsync(It.IsAny<PecaInsumoCatalogo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IPecaInsumoCatalogoRepository> CriarRepository(PecaInsumoCatalogo? item)
    {
        var repository = new Mock<IPecaInsumoCatalogoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        return repository;
    }

    private static RemoverPecaInsumoCatalogoUseCase CriarUseCase(Mock<IPecaInsumoCatalogoRepository> repository)
    {
        return new RemoverPecaInsumoCatalogoUseCase(repository.Object, new RemoverPecaInsumoCatalogoValidator(), MapperFactory.Criar());
    }
}