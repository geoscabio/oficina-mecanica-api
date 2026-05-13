using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ListarOrdensServico;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Builders;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.OrdemServicoUseCases.ListarOrdensServico;

public class ListarOrdensServicoUseCaseTests
{
    [Fact]
    public async Task Dado_OrdensServicoExistentes_Quando_ListarOrdensServico_Entao_DeveRetornarOrdensServico()
    {
        // Arrange
        var ordensServico = new[]
        {
            OrdemServicoTestDataFactory.CriarOrdemServicoRecebida(),
            OrdemServicoTestDataFactory.CriarOrdemServicoFinalizada()
        };
        var repository = new Mock<IOrdemServicoRepository>();
        repository
            .Setup(repo => repo.ListarAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordensServico);
        repository
            .Setup(repo => repo.ContarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordensServico.Length);

        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarOrdensServicoRequest(1, 10));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Pagina.Should().Be(1);
        resultado.Valor.TamanhoPagina.Should().Be(10);
        resultado.Valor.TotalItens.Should().Be(2);
        resultado.Valor.Itens.Should().HaveCount(2);
        resultado.Valor.Itens.Select(ordemServico => ordemServico.Id).Should().BeEquivalentTo(
            ordensServico.Select(ordemServico => ordemServico.Id));
    }

    [Fact]
    public async Task Dado_NenhumaOrdemServico_Quando_ListarOrdensServico_Entao_DeveRetornarListaVazia()
    {
        // Arrange
        var repository = new Mock<IOrdemServicoRepository>();
        repository
            .Setup(repo => repo.ListarAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<OrdemServico>());
        repository
            .Setup(repo => repo.ContarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarOrdensServicoRequest(1, 10));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Itens.Should().BeEmpty();
        resultado.Valor.Pagina.Should().Be(1);
        resultado.Valor.TamanhoPagina.Should().Be(10);
        resultado.Valor.TotalItens.Should().Be(0);
    }

    [Fact]
    public async Task Dado_PaginaSemResultado_Quando_ListarOrdensServico_Entao_DeveRetornarListaVaziaComTotalItens()
    {
        // Arrange
        var repository = new Mock<IOrdemServicoRepository>();
        repository
            .Setup(repo => repo.ListarAsync(2, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<OrdemServico>());
        repository
            .Setup(repo => repo.ContarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarOrdensServicoRequest(2, 10));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Itens.Should().BeEmpty();
        resultado.Valor.Pagina.Should().Be(2);
        resultado.Valor.TamanhoPagina.Should().Be(10);
        resultado.Valor.TotalItens.Should().Be(10);
    }

    [Fact]
    public async Task Dado_PaginaInvalida_Quando_ListarOrdensServico_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IOrdemServicoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarOrdensServicoRequest(0, 10));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    [Fact]
    public async Task Dado_TamanhoPaginaInvalido_Quando_ListarOrdensServico_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IOrdemServicoRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarOrdensServicoRequest(1, 101));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static ListarOrdensServicoUseCase CriarUseCase(Mock<IOrdemServicoRepository> repository)
    {
        return new ListarOrdensServicoUseCase(
            repository.Object,
            new ListarOrdensServicoValidator(),
            MapperFactory.Criar());
    }
}
