using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.ListarItensEstoque;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoEstoque.Factories;
using OficinaMecanica.Domain.GestaoEstoque.Entities;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;

namespace OficinaMecanica.Application.UnitTests.GestaoEstoque.EstoqueUseCases.ListarItensEstoque;

public class ListarItensEstoqueUseCaseTests
{
    [Fact]
    public async Task Dado_ItensExistentes_Quando_ListarItensEstoque_Entao_DeveRetornarItensPaginados()
    {
        // Arrange
        var estoque = EstoqueTestDataFactory.CriarEstoqueComItens(2);

        var itensEstoque = estoque.ItensEstoque.ToArray();

        var repository = CriarRepository(
            itensEstoque,
            itensEstoque.Length);

        var useCase = CriarUseCase(repository);

        var request = EstoqueTestDataFactory.CriarListarItensEstoqueRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();

        resultado.Valor.Should().NotBeNull();

        resultado.Valor!.Pagina.Should().Be(request.Pagina);

        resultado.Valor.TamanhoPagina.Should().Be(request.TamanhoPagina);

        resultado.Valor.TotalItens.Should().Be(itensEstoque.Length);

        resultado.Valor.Itens.Should().HaveCount(itensEstoque.Length);

        resultado.Valor.Itens.Select(item => item.Id).Should().BeEquivalentTo(
            itensEstoque.Select(item => item.Id));

        repository.Verify(
            repo => repo.ListarItensAsync(
                request.Pagina,
                request.TamanhoPagina,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            repo => repo.ContarItensAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_NenhumItem_Quando_ListarItensEstoque_Entao_DeveRetornarListaVazia()
    {
        // Arrange
        var itensEstoque = Array.Empty<ItemEstoque>();

        var repository = CriarRepository(
            itensEstoque,
            totalItens: 0);

        var useCase = CriarUseCase(repository);

        var request = EstoqueTestDataFactory.CriarListarItensEstoqueRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();

        resultado.Valor.Should().NotBeNull();

        resultado.Valor!.Itens.Should().BeEmpty();

        resultado.Valor.Pagina.Should().Be(request.Pagina);

        resultado.Valor.TamanhoPagina.Should().Be(request.TamanhoPagina);

        resultado.Valor.TotalItens.Should().Be(0);
    }

    [Fact]
    public async Task Dado_PaginaInvalida_Quando_ListarItensEstoque_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IEstoqueRepository>();

        var useCase = CriarUseCase(repository);

        var request = EstoqueTestDataFactory.CriarListarItensEstoqueRequestValido(
            pagina: 0);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();

        resultado.Erro.Should().NotBeNull();

        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.ListarItensAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        repository.Verify(
            repo => repo.ContarItensAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_TamanhoPaginaInvalido_Quando_ListarItensEstoque_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IEstoqueRepository>();

        var useCase = CriarUseCase(repository);

        var request = EstoqueTestDataFactory.CriarListarItensEstoqueRequestValido(
            tamanhoPagina: 101);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();

        resultado.Erro.Should().NotBeNull();

        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.ListarItensAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        repository.Verify(
            repo => repo.ContarItensAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Mock<IEstoqueRepository> CriarRepository(
        IReadOnlyCollection<ItemEstoque> itensEstoque,
        int totalItens)
    {
        var repository = new Mock<IEstoqueRepository>();

        repository
            .Setup(repo => repo.ListarItensAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(itensEstoque);

        repository
            .Setup(repo => repo.ContarItensAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalItens);

        return repository;
    }

    private static ListarItensEstoqueUseCase CriarUseCase(
        Mock<IEstoqueRepository> repository)
    {
        return new ListarItensEstoqueUseCase(
            repository.Object,
            new ListarItensEstoqueValidator(),
            MapperFactory.Criar());
    }
}