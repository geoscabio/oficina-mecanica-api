using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.RemoverPecaInsumoCatalogo;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Enums;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;

namespace OficinaMecanica.Application.UnitTests.Administrativo.PecaInsumoCatalogoUseCases.RemoverPecaInsumoCatalogo;

public class RemoverPecaInsumoCatalogoUseCaseTests
{
    [Fact]
    public async Task Dado_PecaInsumoCatalogoExistente_Quando_RemoverPecaInsumoCatalogo_Entao_DeveRemoverPecaInsumoCatalogo()
    {
        // Arrange
        var item = PecaInsumoCatalogo.Criar("Filtro de óleo", TipoPecaInsumo.PECA, 45m);
        var repository = new Mock<IPecaInsumoCatalogoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(item.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new RemoverPecaInsumoCatalogoRequest(item.Id));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(item.Id);

        repository.Verify(
            repo => repo.RemoverAsync(
                It.Is<PecaInsumoCatalogo>(itemRemovido => itemRemovido.Id == item.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_PecaInsumoCatalogoInexistente_Quando_RemoverPecaInsumoCatalogo_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = new Mock<IPecaInsumoCatalogoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new RemoverPecaInsumoCatalogoRequest(Guid.NewGuid()));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(PecaInsumoCatalogoErrorMessages.PecaInsumoCatalogoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(
            repo => repo.RemoverAsync(It.IsAny<PecaInsumoCatalogo>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_RemoverPecaInsumoCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IPecaInsumoCatalogoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new RemoverPecaInsumoCatalogoRequest(Guid.Empty));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static RemoverPecaInsumoCatalogoUseCase CriarUseCase(
        Mock<IPecaInsumoCatalogoRepository> repository)
    {
        return new RemoverPecaInsumoCatalogoUseCase(
            repository.Object,
            new RemoverPecaInsumoCatalogoValidator(),
            MapperFactory.Criar());
    }
}