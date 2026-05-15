using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ListarTempoMedioExecucaoServicos;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Administrativo.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;

namespace OficinaMecanica.Application.UnitTests.Administrativo.ServicoCatalogoUseCases.ListarTempoMedioExecucaoServicos;

public class ListarTempoMedioExecucaoServicosUseCaseTests
{
    [Fact]
    public async Task Dado_ServicosCatalogoComHistorico_Quando_ListarTempoMedioExecucaoServicos_Entao_DeveRetornarServicosComTemposMedios()
    {
        // Arrange
        var servicosCatalogo = new[]
        {
            ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao("Troca de oleo", 150m),
            ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao("Alinhamento", 90m)
        };
        var temposMedios = new Dictionary<Guid, double>
        {
            [servicosCatalogo[0].Id] = 45d
        };
        var servicoCatalogoRepository = CriarServicoCatalogoRepository(servicosCatalogo, totalItens: 2);
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        ordemServicoRepository
            .Setup(repo => repo.ListarTemposMediosExecucaoServicosAsync(
                It.Is<IReadOnlyCollection<Guid>>(ids => ids.Count == 2),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(temposMedios);

        var useCase = CriarUseCase(servicoCatalogoRepository, ordemServicoRepository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarTempoMedioExecucaoServicosRequest(1, 10));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Pagina.Should().Be(1);
        resultado.Valor.TamanhoPagina.Should().Be(10);
        resultado.Valor.TotalItens.Should().Be(2);
        resultado.Valor.Itens.Should().HaveCount(2);
        resultado.Valor.Itens.First().TempoMedioExecucaoEmMinutos.Should().Be(45d);
        resultado.Valor.Itens.Last().TempoMedioExecucaoEmMinutos.Should().BeNull();
    }

    [Fact]
    public async Task Dado_NenhumServicoCatalogo_Quando_ListarTempoMedioExecucaoServicos_Entao_DeveRetornarListaVazia()
    {
        // Arrange
        var servicoCatalogoRepository = CriarServicoCatalogoRepository(Array.Empty<ServicoCatalogo>(), totalItens: 0);
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var useCase = CriarUseCase(servicoCatalogoRepository, ordemServicoRepository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarTempoMedioExecucaoServicosRequest(1, 10));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Itens.Should().BeEmpty();
        resultado.Valor.TotalItens.Should().Be(0);
        ordemServicoRepository.Verify(
            repo => repo.ListarTemposMediosExecucaoServicosAsync(
                It.IsAny<IReadOnlyCollection<Guid>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_PaginaInvalida_Quando_ListarTempoMedioExecucaoServicos_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var servicoCatalogoRepository = new Mock<IServicoCatalogoRepository>();
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var useCase = CriarUseCase(servicoCatalogoRepository, ordemServicoRepository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarTempoMedioExecucaoServicosRequest(0, 10));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    [Fact]
    public async Task Dado_TamanhoPaginaInvalido_Quando_ListarTempoMedioExecucaoServicos_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var servicoCatalogoRepository = new Mock<IServicoCatalogoRepository>();
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var useCase = CriarUseCase(servicoCatalogoRepository, ordemServicoRepository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ListarTempoMedioExecucaoServicosRequest(1, 101));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static Mock<IServicoCatalogoRepository> CriarServicoCatalogoRepository(
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

    private static ListarTempoMedioExecucaoServicosUseCase CriarUseCase(
        Mock<IServicoCatalogoRepository> servicoCatalogoRepository,
        Mock<IOrdemServicoRepository> ordemServicoRepository)
    {
        return new ListarTempoMedioExecucaoServicosUseCase(
            servicoCatalogoRepository.Object,
            ordemServicoRepository.Object,
            new ListarTempoMedioExecucaoServicosValidator(),
            MapperFactory.Criar());
    }
}
