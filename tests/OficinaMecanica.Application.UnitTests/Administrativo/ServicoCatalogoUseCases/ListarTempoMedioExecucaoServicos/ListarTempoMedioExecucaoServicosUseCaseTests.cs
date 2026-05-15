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
    public async Task Dado_ServicosCatalogoExistentes_Quando_ListarTempoMedioExecucaoServicos_Entao_DeveRetornarTemposMedios()
    {
        // Arrange
        var servicosCatalogo = new[]
        {
            ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao(),
            ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao(
                ServicoCatalogoTestDataFactory.DescricaoAtualizada,
                ServicoCatalogoTestDataFactory.ValorAtualizado)
        };

        var temposMedios = new Dictionary<Guid, double>
        {
            { servicosCatalogo[0].Id, ServicoCatalogoTestDataFactory.TempoMedioExecucaoPadrao }
        };

        var servicoCatalogoRepository = CriarServicoCatalogoRepository(
            servicosCatalogo,
            totalItens: servicosCatalogo.Length);

        var ordemServicoRepository = CriarOrdemServicoRepository(temposMedios);

        var useCase = CriarUseCase(servicoCatalogoRepository, ordemServicoRepository);

        var request = ServicoCatalogoTestDataFactory.CriarListarTempoMedioExecucaoServicosRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Pagina.Should().Be(ServicoCatalogoTestDataFactory.PaginaPadrao);
        resultado.Valor.TamanhoPagina.Should().Be(ServicoCatalogoTestDataFactory.TamanhoPaginaPadrao);
        resultado.Valor.TotalItens.Should().Be(servicosCatalogo.Length);
        resultado.Valor.Itens.Should().HaveCount(servicosCatalogo.Length);

        resultado.Valor.Itens.Select(servico => servico.ServicoCatalogoId).Should().BeEquivalentTo(
            servicosCatalogo.Select(servico => servico.Id));

        resultado.Valor.Itens
            .Single(servico => servico.ServicoCatalogoId == servicosCatalogo[0].Id)
            .TempoMedioExecucaoEmMinutos
            .Should()
            .Be(ServicoCatalogoTestDataFactory.TempoMedioExecucaoPadrao);

        resultado.Valor.Itens
            .Single(servico => servico.ServicoCatalogoId == servicosCatalogo[1].Id)
            .TempoMedioExecucaoEmMinutos
            .Should()
            .BeNull();

        servicoCatalogoRepository.Verify(
            repo => repo.ListarAsync(
                ServicoCatalogoTestDataFactory.PaginaPadrao,
                ServicoCatalogoTestDataFactory.TamanhoPaginaPadrao,
                It.IsAny<CancellationToken>()),
            Times.Once);

        servicoCatalogoRepository.Verify(
            repo => repo.ContarAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        ordemServicoRepository.Verify(
            repo => repo.ListarTemposMediosExecucaoServicosAsync(
                It.Is<IReadOnlyCollection<Guid>>(ids =>
                    ids.Count == servicosCatalogo.Length
                    && servicosCatalogo.All(servico => ids.Contains(servico.Id))),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_NenhumServicoCatalogo_Quando_ListarTempoMedioExecucaoServicos_Entao_DeveRetornarListaVazia()
    {
        // Arrange
        var servicoCatalogoRepository = CriarServicoCatalogoRepository(
            Array.Empty<ServicoCatalogo>(),
            totalItens: 0);

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();

        var useCase = CriarUseCase(servicoCatalogoRepository, ordemServicoRepository);

        var request = ServicoCatalogoTestDataFactory.CriarListarTempoMedioExecucaoServicosRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Itens.Should().BeEmpty();
        resultado.Valor.Pagina.Should().Be(ServicoCatalogoTestDataFactory.PaginaPadrao);
        resultado.Valor.TamanhoPagina.Should().Be(ServicoCatalogoTestDataFactory.TamanhoPaginaPadrao);
        resultado.Valor.TotalItens.Should().Be(0);

        servicoCatalogoRepository.Verify(
            repo => repo.ListarAsync(
                ServicoCatalogoTestDataFactory.PaginaPadrao,
                ServicoCatalogoTestDataFactory.TamanhoPaginaPadrao,
                It.IsAny<CancellationToken>()),
            Times.Once);

        servicoCatalogoRepository.Verify(
            repo => repo.ContarAsync(It.IsAny<CancellationToken>()),
            Times.Once);

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

        var request = ServicoCatalogoTestDataFactory.CriarListarTempoMedioExecucaoServicosRequestValido(
            pagina: 0);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        servicoCatalogoRepository.Verify(
            repo => repo.ListarAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        servicoCatalogoRepository.Verify(
            repo => repo.ContarAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        ordemServicoRepository.Verify(
            repo => repo.ListarTemposMediosExecucaoServicosAsync(
                It.IsAny<IReadOnlyCollection<Guid>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_TamanhoPaginaInvalido_Quando_ListarTempoMedioExecucaoServicos_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var servicoCatalogoRepository = new Mock<IServicoCatalogoRepository>();

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();

        var useCase = CriarUseCase(servicoCatalogoRepository, ordemServicoRepository);

        var request = ServicoCatalogoTestDataFactory.CriarListarTempoMedioExecucaoServicosRequestValido(
            tamanhoPagina: 101);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        servicoCatalogoRepository.Verify(
            repo => repo.ListarAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        servicoCatalogoRepository.Verify(
            repo => repo.ContarAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        ordemServicoRepository.Verify(
            repo => repo.ListarTemposMediosExecucaoServicosAsync(
                It.IsAny<IReadOnlyCollection<Guid>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Mock<IServicoCatalogoRepository> CriarServicoCatalogoRepository(
        IReadOnlyCollection<ServicoCatalogo> servicosCatalogo,
        int totalItens)
    {
        var repository = new Mock<IServicoCatalogoRepository>();

        repository
            .Setup(repo => repo.ListarAsync(
                ServicoCatalogoTestDataFactory.PaginaPadrao,
                ServicoCatalogoTestDataFactory.TamanhoPaginaPadrao,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(servicosCatalogo);

        repository
            .Setup(repo => repo.ContarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(totalItens);

        return repository;
    }

    private static Mock<IOrdemServicoRepository> CriarOrdemServicoRepository(
        IReadOnlyDictionary<Guid, double> temposMedios)
    {
        var repository = new Mock<IOrdemServicoRepository>();

        repository
            .Setup(repo => repo.ListarTemposMediosExecucaoServicosAsync(
                It.IsAny<IReadOnlyCollection<Guid>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(temposMedios);

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