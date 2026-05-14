using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.ListarItensEstoque;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoEstoque.Builders;
using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;

namespace OficinaMecanica.Application.UnitTests.GestaoEstoque.EstoqueUseCases.ListarItensEstoque;

public class ListarItensEstoqueUseCaseTests
{
    [Fact]
    public async Task Dado_ItensExistentes_Quando_ListarItensEstoque_Entao_DeveRetornarItens()
    {
        // Arrange
        var estoque = EstoqueTestDataFactory.CriarEstoqueComItens(2);
        var repository = CriarRepository(estoque);
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarItensEstoqueRequest(1, 10));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Pagina.Should().Be(1);
        resultado.Valor.TamanhoPagina.Should().Be(10);
        resultado.Valor.TotalItens.Should().Be(2);
        resultado.Valor.Itens.Should().HaveCount(2);
        resultado.Valor.Itens.Select(item => item.Id).Should().BeEquivalentTo(
            estoque.ItensEstoque.Select(item => item.Id));
    }

    [Fact]
    public async Task Dado_NenhumItem_Quando_ListarItensEstoque_Entao_DeveRetornarListaVazia()
    {
        // Arrange
        var repository = CriarRepository(null);
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarItensEstoqueRequest(1, 10));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Itens.Should().BeEmpty();
        resultado.Valor.Pagina.Should().Be(1);
        resultado.Valor.TamanhoPagina.Should().Be(10);
        resultado.Valor.TotalItens.Should().Be(0);
    }

    [Fact]
    public async Task Dado_PaginaInvalida_Quando_ListarItensEstoque_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IEstoqueRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarItensEstoqueRequest(0, 10));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    [Fact]
    public async Task Dado_TamanhoPaginaInvalido_Quando_ListarItensEstoque_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IEstoqueRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarItensEstoqueRequest(1, 101));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static Mock<IEstoqueRepository> CriarRepository(Estoque? estoque)
    {
        var repository = new Mock<IEstoqueRepository>();

        repository
            .Setup(repo => repo.ObterAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(estoque);

        return repository;
    }

    private static ListarItensEstoqueUseCase CriarUseCase(Mock<IEstoqueRepository> repository)
    {
        return new ListarItensEstoqueUseCase(
            repository.Object,
            new ListarItensEstoqueValidator(),
            MapperFactory.Criar());
    }
}