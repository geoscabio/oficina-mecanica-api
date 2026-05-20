using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.DefinirServicos;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Factories;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;

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

        var ordemServicoRepository = CriarOrdemServicoRepository(ordemServico);

        var servicoCatalogoRepository = CriarServicoCatalogoRepository(trocaOleo, alinhamento);

        var useCase = CriarUseCase(ordemServicoRepository, servicoCatalogoRepository);

        var request = OrdemServicoTestDataFactory.CriarDefinirServicosRequestValido(ordemServico.Id, new[] { trocaOleo.Id, alinhamento.Id });

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(ordemServico.Id);
        resultado.Valor.ValorTotal.Should().Be(230m);
        resultado.Valor.Status.Should().Be(OrdemServicoTestDataFactory.StatusEmDiagnostico);
        resultado.Valor.Servicos.Should().HaveCount(2);
        resultado.Valor.Servicos.Should().Contain(servico =>
            servico.ServicoCatalogoId == trocaOleo.Id
            && servico.Valor == trocaOleo.Valor
            && servico.Status == OrdemServicoTestDataFactory.StatusPendente);
        resultado.Valor.Servicos.Should().Contain(servico =>
            servico.ServicoCatalogoId == alinhamento.Id
            && servico.Valor == alinhamento.Valor
            && servico.Status == OrdemServicoTestDataFactory.StatusPendente);
        resultado.Valor.PecasInsumos.Should().BeEmpty();

        ordemServicoRepository.Verify(repo => repo.ObterPorIdAsync(request.OrdemServicoId, It.IsAny<CancellationToken>()), Times.Once);

        servicoCatalogoRepository.Verify(repo => repo.ObterPorIdsAsync(It.Is<IReadOnlyCollection<Guid>>(ids => ids.Contains(trocaOleo.Id) && ids.Contains(alinhamento.Id) && ids.Count == 2), It.IsAny<CancellationToken>()), Times.Once);

        servicoCatalogoRepository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        ordemServicoRepository.Verify(
            repo => repo.AtualizarAsync(
                It.Is<OrdemServico>(ordemServicoAtualizada =>
                    ordemServicoAtualizada.Id == ordemServico.Id
                    && ordemServicoAtualizada.Servicos.Count == 2
                    && ordemServicoAtualizada.Servicos.Any(servico => servico.ServicoCatalogoId == trocaOleo.Id)
                    && ordemServicoAtualizada.Servicos.Any(servico => servico.ServicoCatalogoId == alinhamento.Id)
                    && ordemServicoAtualizada.ValorTotal == 230m),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_OrdemServicoInexistente_Quando_DefinirServicos_Entao_DeveRetornarFalha()
    {
        // Arrange
        var servicoCatalogo = ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao();

        var ordemServicoRepository = CriarOrdemServicoRepository(null);

        var servicoCatalogoRepository = new Mock<IServicoCatalogoRepository>();

        var useCase = CriarUseCase(ordemServicoRepository, servicoCatalogoRepository);

        var request = OrdemServicoTestDataFactory.CriarDefinirServicosRequestValido(servicosCatalogoIds: new[] { servicoCatalogo.Id });

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(OrdemServicoErrorMessages.OrdemServicoNaoEncontrada);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        ordemServicoRepository.Verify(repo => repo.ObterPorIdAsync(request.OrdemServicoId, It.IsAny<CancellationToken>()), Times.Once);

        servicoCatalogoRepository.Verify(repo => repo.ObterPorIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);

        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_AlgumServicoCatalogoInexistente_Quando_DefinirServicos_Entao_DeveRetornarFalhaSemAlterarOrdemServico()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnostico();

        var servicoCatalogo = ServicoCatalogoTestDataFactory.CriarServicoCatalogoPadrao();

        var servicoCatalogoInexistenteId = Guid.NewGuid();

        var ordemServicoRepository = CriarOrdemServicoRepository(ordemServico);

        var servicoCatalogoRepository = CriarServicoCatalogoRepository(servicoCatalogo);

        var useCase = CriarUseCase(ordemServicoRepository, servicoCatalogoRepository);

        var request = OrdemServicoTestDataFactory.CriarDefinirServicosRequestValido(ordemServico.Id, new[] { servicoCatalogo.Id, servicoCatalogoInexistenteId });

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(ServicoCatalogoErrorMessages.ServicoCatalogoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        ordemServico.Servicos.Should().BeEmpty();

        ordemServicoRepository.Verify(repo => repo.ObterPorIdAsync(request.OrdemServicoId, It.IsAny<CancellationToken>()), Times.Once);

        servicoCatalogoRepository.Verify(
            repo => repo.ObterPorIdsAsync(It.Is<IReadOnlyCollection<Guid>>(ids => ids.Contains(servicoCatalogo.Id) && ids.Contains(servicoCatalogoInexistenteId) && ids.Count == 2), It.IsAny<CancellationToken>()),
            Times.Once);

        servicoCatalogoRepository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_DefinirServicos_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();

        var servicoCatalogoRepository = new Mock<IServicoCatalogoRepository>();

        var useCase = CriarUseCase(ordemServicoRepository, servicoCatalogoRepository);

        var request = OrdemServicoTestDataFactory.CriarDefinirServicosRequestValido(ordemServicoId: Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        ordemServicoRepository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        servicoCatalogoRepository.Verify(repo => repo.ObterPorIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);

        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_ListaServicosVazia_Quando_DefinirServicos_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();

        var servicoCatalogoRepository = new Mock<IServicoCatalogoRepository>();

        var useCase = CriarUseCase(ordemServicoRepository, servicoCatalogoRepository);

        var request = OrdemServicoTestDataFactory.CriarDefinirServicosRequestValido(servicosCatalogoIds: Array.Empty<Guid>());

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        ordemServicoRepository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        servicoCatalogoRepository.Verify(repo => repo.ObterPorIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);

        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_ServicoCatalogoIdVazio_Quando_DefinirServicos_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();

        var servicoCatalogoRepository = new Mock<IServicoCatalogoRepository>();

        var useCase = CriarUseCase(ordemServicoRepository, servicoCatalogoRepository);

        var request = OrdemServicoTestDataFactory.CriarDefinirServicosRequestValido(servicosCatalogoIds: new[] { Guid.Empty });

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        ordemServicoRepository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        servicoCatalogoRepository.Verify(repo => repo.ObterPorIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);

        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IOrdemServicoRepository> CriarOrdemServicoRepository(OrdemServico? ordemServico)
    {
        var repository = new Mock<IOrdemServicoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        return repository;
    }

    private static Mock<IServicoCatalogoRepository> CriarServicoCatalogoRepository(params ServicoCatalogo[] servicosCatalogo)
    {
        var repository = new Mock<IServicoCatalogoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdsAsync(It.IsAny<IReadOnlyCollection<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<Guid> ids, CancellationToken _) => servicosCatalogo.Where(servicoCatalogo => ids.Contains(servicoCatalogo.Id)).ToArray());

        return repository;
    }

    private static DefinirServicosUseCase CriarUseCase(Mock<IOrdemServicoRepository> ordemServicoRepository, Mock<IServicoCatalogoRepository> servicoCatalogoRepository)
    {
        return new DefinirServicosUseCase(ordemServicoRepository.Object, servicoCatalogoRepository.Object, new DefinirServicosValidator(), MapperFactory.Criar());
    }
}
