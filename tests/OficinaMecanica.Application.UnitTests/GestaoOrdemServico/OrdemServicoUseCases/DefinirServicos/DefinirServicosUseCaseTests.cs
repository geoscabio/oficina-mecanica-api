using FluentAssertions;
using FluentValidation;
using Moq;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.DefinirServicos;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Builders;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.OrdemServicoUseCases.DefinirServicos;

public class DefinirServicosUseCaseTests
{
    [Fact]
    public async Task Dado_OrdemServicoEmDiagnosticoEServicosCatalogoExistentes_Quando_DefinirServicos_Entao_DeveAdicionarServicosEAtualizarOrcamento()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnostico();
        var trocaOleo = ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao();
        var alinhamento = ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao("Alinhamento", 80m);
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var servicoCatalogoRepository = new Mock<IServicoCatalogoRepository>();

        ordemServicoRepository
            .Setup(repo => repo.ObterPorIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        ConfigurarServicoCatalogo(servicoCatalogoRepository, trocaOleo);
        ConfigurarServicoCatalogo(servicoCatalogoRepository, alinhamento);

        var useCase = CriarUseCase(ordemServicoRepository, servicoCatalogoRepository);
        var request = new DefinirServicosRequest(
            ordemServico.Id,
            new[] { trocaOleo.Id, alinhamento.Id });

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(ordemServico.Id);
        resultado.Valor.ValorTotal.Should().Be(230m);
        resultado.Valor.Status.Should().Be("EM_DIAGNOSTICO");

        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.Is<OrdemServico>(ordemServicoAtualizada =>
            ordemServicoAtualizada.Id == ordemServico.Id
            && ordemServicoAtualizada.Servicos.Count == 2
            && ordemServicoAtualizada.Servicos.Any(servico => servico.ServicoCatalogoId == trocaOleo.Id)
            && ordemServicoAtualizada.Servicos.Any(servico => servico.ServicoCatalogoId == alinhamento.Id)
            && ordemServicoAtualizada.ValorTotal == 230m), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_OrdemServicoInexistente_Quando_DefinirServicos_Entao_DeveRetornarFalha()
    {
        // Arrange
        var servicoCatalogo = ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao();
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var servicoCatalogoRepository = new Mock<IServicoCatalogoRepository>();
        var useCase = CriarUseCase(ordemServicoRepository, servicoCatalogoRepository);
        var request = new DefinirServicosRequest(Guid.NewGuid(), new[] { servicoCatalogo.Id });

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be("Ordem de servico nao encontrada.");

        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_AlgumServicoCatalogoInexistente_Quando_DefinirServicos_Entao_DeveRetornarFalhaSemAlterarOrdemServico()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnostico();
        var servicoCatalogo = ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao();
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var servicoCatalogoRepository = new Mock<IServicoCatalogoRepository>();

        ordemServicoRepository
            .Setup(repo => repo.ObterPorIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        ConfigurarServicoCatalogo(servicoCatalogoRepository, servicoCatalogo);

        var useCase = CriarUseCase(ordemServicoRepository, servicoCatalogoRepository);
        var request = new DefinirServicosRequest(
            ordemServico.Id,
            new[] { servicoCatalogo.Id, Guid.NewGuid() });

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be("Servico do catalogo nao encontrado.");
        ordemServico.Servicos.Should().BeEmpty();

        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_ListaServicosVazia_Quando_DefinirServicos_Entao_DeveLancarValidationException()
    {
        // Arrange
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var servicoCatalogoRepository = new Mock<IServicoCatalogoRepository>();
        var useCase = CriarUseCase(ordemServicoRepository, servicoCatalogoRepository);
        var request = new DefinirServicosRequest(Guid.NewGuid(), Array.Empty<Guid>());

        // Act
        var acao = () => useCase.ExecuteAsync(request);

        // Assert
        await acao.Should().ThrowAsync<ValidationException>();
    }

    private static DefinirServicosUseCase CriarUseCase(
        Mock<IOrdemServicoRepository> ordemServicoRepository,
        Mock<IServicoCatalogoRepository> servicoCatalogoRepository)
    {
        return new DefinirServicosUseCase(
            ordemServicoRepository.Object,
            servicoCatalogoRepository.Object,
            new DefinirServicosValidator(),
            MapperFactory.Criar());
    }

    private static void ConfigurarServicoCatalogo(
        Mock<IServicoCatalogoRepository> repository,
        ServicoCatalogo servicoCatalogo)
    {
        repository
            .Setup(repo => repo.ObterPorIdAsync(servicoCatalogo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(servicoCatalogo);
    }
}


