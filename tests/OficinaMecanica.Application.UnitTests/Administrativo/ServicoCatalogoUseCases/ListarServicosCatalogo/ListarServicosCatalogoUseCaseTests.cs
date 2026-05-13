using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ListarServicosCatalogo;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Builders;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;

namespace OficinaMecanica.Application.UnitTests.Administrativo.ServicoCatalogoUseCases.ListarServicosCatalogo;

public class ListarServicosCatalogoUseCaseTests
{
    [Fact]
    public async Task Dado_ServicosCatalogoExistentes_Quando_ListarServicosCatalogo_Entao_DeveRetornarServicosCatalogo()
    {
        // Arrange
        var servicosCatalogo = new[]
        {
            ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao("Troca de óleo", 150m),
            ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao("Alinhamento", 90m)
        };
        var repository = CriarRepository(servicosCatalogo, totalItens: 2);
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarServicosCatalogoRequest(1, 10));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Pagina.Should().Be(1);
        resultado.Valor.TamanhoPagina.Should().Be(10);
        resultado.Valor.TotalItens.Should().Be(2);
        resultado.Valor.Itens.Should().HaveCount(2);
        resultado.Valor.Itens.Select(servicoCatalogo => servicoCatalogo.Id).Should().BeEquivalentTo(
            servicosCatalogo.Select(servicoCatalogo => servicoCatalogo.Id));
    }

    [Fact]
    public async Task Dado_NenhumServicoCatalogo_Quando_ListarServicosCatalogo_Entao_DeveRetornarListaVazia()
    {
        // Arrange
        var repository = CriarRepository(Array.Empty<ServicoCatalogo>(), totalItens: 0);
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarServicosCatalogoRequest(1, 10));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Itens.Should().BeEmpty();
        resultado.Valor.Pagina.Should().Be(1);
        resultado.Valor.TamanhoPagina.Should().Be(10);
        resultado.Valor.TotalItens.Should().Be(0);
    }

    [Fact]
    public async Task Dado_PaginaInvalida_Quando_ListarServicosCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IServicoCatalogoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarServicosCatalogoRequest(0, 10));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    [Fact]
    public async Task Dado_TamanhoPaginaInvalido_Quando_ListarServicosCatalogo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IServicoCatalogoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarServicosCatalogoRequest(1, 101));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static Mock<IServicoCatalogoRepository> CriarRepository(
        IReadOnlyCollection<ServicoCatalogo> servicosCatalogo,
        int totalItens)
    {
        var repository = new Mock<IServicoCatalogoRepository>();
        repository
            .Setup(repo => repo.ListarAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(servicosCatalogo);
        repository
            .Setup(repo => repo.ContarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalItens);

        return repository;
    }

    private static ListarServicosCatalogoUseCase CriarUseCase(Mock<IServicoCatalogoRepository> repository)
    {
        return new ListarServicosCatalogoUseCase(
            repository.Object,
            new ListarServicosCatalogoValidator(),
            MapperFactory.Criar());
    }
}
