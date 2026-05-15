using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.ListarPecasInsumosCatalogo;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;

namespace OficinaMecanica.Application.UnitTests.Administrativo.PecaInsumoCatalogoUseCases.ListarPecasInsumosCatalogo;

public class ListarPecasInsumosCatalogoUseCaseTests
{
    [Fact]
    public async Task Dado_ItensExistentes_Quando_ListarPecasInsumosCatalogo_Entao_DeveRetornarItens()
    {
        // Arrange
        var itens = new[]
        {
            PecaInsumoCatalogoTestDataFactory.CriarPecaInsumoCatalogoPadrao(),
            PecaInsumoCatalogoTestDataFactory.CriarPecaInsumoCatalogoPadrao(
                PecaInsumoCatalogoTestDataFactory.DescricaoAtualizada,
                PecaInsumoCatalogoTestDataFactory.TipoAtualizado,
                PecaInsumoCatalogoTestDataFactory.ValorAtualizado)
        };

        var repository = CriarRepository(itens, totalItens: itens.Length);

        var useCase = CriarUseCase(repository);

        var request = PecaInsumoCatalogoTestDataFactory.CriarListarPecasInsumosCatalogoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Pagina.Should().Be(PecaInsumoCatalogoTestDataFactory.PaginaPadrao);
        resultado.Valor.TamanhoPagina.Should().Be(PecaInsumoCatalogoTestDataFactory.TamanhoPaginaPadrao);
        resultado.Valor.TotalItens.Should().Be(itens.Length);
        resultado.Valor.Itens.Should().HaveCount(itens.Length);
        resultado.Valor.Itens.Select(item => item.Id).Should().BeEquivalentTo(
            itens.Select(item => item.Id));

        repository.Verify(
            repo => repo.ListarAsync(
                PecaInsumoCatalogoTestDataFactory.PaginaPadrao,
                PecaInsumoCatalogoTestDataFactory.TamanhoPaginaPadrao,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            repo => repo.ContarAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_NenhumItem_Quando_ListarPecasInsumosCatalogo_Entao_DeveRetornarListaVazia()
    {
        // Arrange
        var repository = CriarRepository(Array.Empty<PecaInsumoCatalogo>(), totalItens: 0);

        var useCase = CriarUseCase(repository);

        var request = PecaInsumoCatalogoTestDataFactory.CriarListarPecasInsumosCatalogoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Itens.Should().BeEmpty();
        resultado.Valor.Pagina.Should().Be(PecaInsumoCatalogoTestDataFactory.PaginaPadrao);
        resultado.Valor.TamanhoPagina.Should().Be(PecaInsumoCatalogoTestDataFactory.TamanhoPaginaPadrao);
        resultado.Valor.TotalItens.Should().Be(0);

        repository.Verify(
            repo => repo.ListarAsync(
                PecaInsumoCatalogoTestDataFactory.PaginaPadrao,
                PecaInsumoCatalogoTestDataFactory.TamanhoPaginaPadrao,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            repo => repo.ContarAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_PaginaInvalida_Quando_ListarPecasInsumosCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IPecaInsumoCatalogoRepository>();

        var useCase = CriarUseCase(repository);

        var request = PecaInsumoCatalogoTestDataFactory.CriarListarPecasInsumosCatalogoRequestValido(
            pagina: 0);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.ListarAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        repository.Verify(
            repo => repo.ContarAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_TamanhoPaginaInvalido_Quando_ListarPecasInsumosCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IPecaInsumoCatalogoRepository>();

        var useCase = CriarUseCase(repository);

        var request = PecaInsumoCatalogoTestDataFactory.CriarListarPecasInsumosCatalogoRequestValido(
            tamanhoPagina: 101);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.ListarAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        repository.Verify(
            repo => repo.ContarAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Mock<IPecaInsumoCatalogoRepository> CriarRepository(
        IReadOnlyCollection<PecaInsumoCatalogo> itens,
        int totalItens)
    {
        var repository = new Mock<IPecaInsumoCatalogoRepository>();

        repository
            .Setup(repo => repo.ListarAsync(
                PecaInsumoCatalogoTestDataFactory.PaginaPadrao,
                PecaInsumoCatalogoTestDataFactory.TamanhoPaginaPadrao,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(itens);

        repository
            .Setup(repo => repo.ContarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalItens);

        return repository;
    }

    private static ListarPecasInsumosCatalogoUseCase CriarUseCase(
        Mock<IPecaInsumoCatalogoRepository> repository)
    {
        return new ListarPecasInsumosCatalogoUseCase(
            repository.Object,
            new ListarPecasInsumosCatalogoValidator(),
            MapperFactory.Criar());
    }
}