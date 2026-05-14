using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.ListarPecasInsumosCatalogo;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Enums;
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
            PecaInsumoCatalogo.Criar("Filtro de óleo", TipoPecaInsumo.PECA, 45m),
            PecaInsumoCatalogo.Criar("Óleo 5W30", TipoPecaInsumo.INSUMO, 38m)
        };

        var repository = CriarRepository(itens, totalItens: 2);
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarPecasInsumosCatalogoRequest(1, 10));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Pagina.Should().Be(1);
        resultado.Valor.TamanhoPagina.Should().Be(10);
        resultado.Valor.TotalItens.Should().Be(2);
        resultado.Valor.Itens.Should().HaveCount(2);
        resultado.Valor.Itens.Select(item => item.Id).Should().BeEquivalentTo(
            itens.Select(item => item.Id));
    }

    [Fact]
    public async Task Dado_NenhumItem_Quando_ListarPecasInsumosCatalogo_Entao_DeveRetornarListaVazia()
    {
        // Arrange
        var repository = CriarRepository(Array.Empty<PecaInsumoCatalogo>(), totalItens: 0);
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarPecasInsumosCatalogoRequest(1, 10));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Itens.Should().BeEmpty();
        resultado.Valor.Pagina.Should().Be(1);
        resultado.Valor.TamanhoPagina.Should().Be(10);
        resultado.Valor.TotalItens.Should().Be(0);
    }

    [Fact]
    public async Task Dado_PaginaInvalida_Quando_ListarPecasInsumosCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IPecaInsumoCatalogoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarPecasInsumosCatalogoRequest(0, 10));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    [Fact]
    public async Task Dado_TamanhoPaginaInvalido_Quando_ListarPecasInsumosCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IPecaInsumoCatalogoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarPecasInsumosCatalogoRequest(1, 101));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static Mock<IPecaInsumoCatalogoRepository> CriarRepository(
        IReadOnlyCollection<PecaInsumoCatalogo> itens,
        int totalItens)
    {
        var repository = new Mock<IPecaInsumoCatalogoRepository>();

        repository
            .Setup(repo => repo.ListarAsync(1, 10, It.IsAny<CancellationToken>()))
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