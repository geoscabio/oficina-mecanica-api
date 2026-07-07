using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ListarOrdensServico;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Factories;
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
            OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnostico()
        };

        var repository = CriarRepository(ordensServico, totalItens: ordensServico.Length);

        var useCase = CriarUseCase(repository);

        var request = OrdemServicoTestDataFactory.CriarListarOrdensServicoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Pagina.Should().Be(OrdemServicoTestDataFactory.PaginaPadrao);
        resultado.Valor.TamanhoPagina.Should().Be(OrdemServicoTestDataFactory.TamanhoPaginaPadrao);
        resultado.Valor.TotalItens.Should().Be(ordensServico.Length);
        resultado.Valor.Itens.Should().HaveCount(ordensServico.Length);
        resultado.Valor.Itens.Select(ordemServico => ordemServico.Id).Should().BeEquivalentTo(ordensServico.Select(ordemServico => ordemServico.Id));

        repository.Verify(repo => repo.ListarAsync(OrdemServicoTestDataFactory.PaginaPadrao, OrdemServicoTestDataFactory.TamanhoPaginaPadrao, It.IsAny<CancellationToken>()), Times.Once);

        repository.Verify(repo => repo.ContarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_NenhumaOrdemServico_Quando_ListarOrdensServico_Entao_DeveRetornarListaVazia()
    {
        // Arrange
        var repository = CriarRepository(Array.Empty<OrdemServico>(), totalItens: 0);

        var useCase = CriarUseCase(repository);

        var request = OrdemServicoTestDataFactory.CriarListarOrdensServicoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Itens.Should().BeEmpty();
        resultado.Valor.Pagina.Should().Be(OrdemServicoTestDataFactory.PaginaPadrao);
        resultado.Valor.TamanhoPagina.Should().Be(OrdemServicoTestDataFactory.TamanhoPaginaPadrao);
        resultado.Valor.TotalItens.Should().Be(0);

        repository.Verify(repo => repo.ListarAsync(OrdemServicoTestDataFactory.PaginaPadrao, OrdemServicoTestDataFactory.TamanhoPaginaPadrao, It.IsAny<CancellationToken>()), Times.Once);

        repository.Verify(repo => repo.ContarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_PaginaSemResultado_Quando_ListarOrdensServico_Entao_DeveRetornarListaVaziaComTotalItens()
    {
        // Arrange
        var repository = CriarRepository(Array.Empty<OrdemServico>(), totalItens: 10, pagina: OrdemServicoTestDataFactory.SegundaPagina);

        var useCase = CriarUseCase(repository);

        var request = OrdemServicoTestDataFactory.CriarListarOrdensServicoRequestValido(pagina: OrdemServicoTestDataFactory.SegundaPagina);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Itens.Should().BeEmpty();
        resultado.Valor.Pagina.Should().Be(OrdemServicoTestDataFactory.SegundaPagina);
        resultado.Valor.TamanhoPagina.Should().Be(OrdemServicoTestDataFactory.TamanhoPaginaPadrao);
        resultado.Valor.TotalItens.Should().Be(10);

        repository.Verify(repo => repo.ListarAsync(OrdemServicoTestDataFactory.SegundaPagina, OrdemServicoTestDataFactory.TamanhoPaginaPadrao, It.IsAny<CancellationToken>()), Times.Once);

        repository.Verify(repo => repo.ContarAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_PaginaInvalida_Quando_ListarOrdensServico_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IOrdemServicoRepository>();

        var useCase = CriarUseCase(repository);

        var request = OrdemServicoTestDataFactory.CriarListarOrdensServicoRequestValido(pagina: 0);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ListarAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.ContarAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_TamanhoPaginaInvalido_Quando_ListarOrdensServico_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IOrdemServicoRepository>();

        var useCase = CriarUseCase(repository);

        var request = OrdemServicoTestDataFactory.CriarListarOrdensServicoRequestValido(tamanhoPagina: 101);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ListarAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.ContarAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IOrdemServicoRepository> CriarRepository(
        IReadOnlyCollection<OrdemServico> ordensServico,
        int totalItens,
        int pagina = OrdemServicoTestDataFactory.PaginaPadrao,
        int tamanhoPagina = OrdemServicoTestDataFactory.TamanhoPaginaPadrao)
    {
        var repository = new Mock<IOrdemServicoRepository>();

        repository
            .Setup(repo => repo.ListarAsync(pagina, tamanhoPagina, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordensServico);

        repository
            .Setup(repo => repo.ContarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalItens);

        return repository;
    }

    private static ListarOrdensServicoUseCase CriarUseCase(Mock<IOrdemServicoRepository> repository)
    {
        return new ListarOrdensServicoUseCase(repository.Object, new ListarOrdensServicoValidator(), MapperFactory.Criar());
    }
}
