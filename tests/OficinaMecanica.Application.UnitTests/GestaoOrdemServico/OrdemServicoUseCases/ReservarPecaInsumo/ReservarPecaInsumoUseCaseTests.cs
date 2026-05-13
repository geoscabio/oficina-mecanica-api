using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.GestaoEstoque.Messages;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;
using OficinaMecanica.Domain.Administrativo.Messages;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ReservarPecaInsumo;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Builders;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;
using OficinaMecanica.Domain.GestaoEstoque.Aggregates;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.OrdemServicoUseCases.ReservarPecaInsumo;

public class ReservarPecaInsumoUseCaseTests
{
    [Fact]
    public async Task Dado_OrdemServicoEmDiagnosticoPecaCatalogoEEstoqueDisponivel_Quando_ReservarPecaInsumo_Entao_DeveReservarEstoqueEAtualizarOrcamento()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnostico();
        var pecaInsumoCatalogo = PecaInsumoCatalogoTestDataFactory.CriarPecaInsumoCatalogoPadrao();
        var estoque = EstoqueTestDataFactory.CriarEstoqueComItem(pecaInsumoCatalogo.Id);
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var pecaInsumoCatalogoRepository = new Mock<IPecaInsumoCatalogoRepository>();
        var estoqueRepository = new Mock<IEstoqueRepository>();

        ordemServicoRepository
            .Setup(repo => repo.ObterPorIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        ConfigurarPecaInsumoCatalogo(pecaInsumoCatalogoRepository, pecaInsumoCatalogo);

        estoqueRepository
            .Setup(repo => repo.ObterAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(estoque);

        var useCase = CriarUseCase(ordemServicoRepository, pecaInsumoCatalogoRepository, estoqueRepository);
        var request = new ReservarPecaInsumoRequest(
            ordemServico.Id,
            new[] { new PecaInsumoRequest(pecaInsumoCatalogo.Id, 2) });

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(ordemServico.Id);
        resultado.Valor.ValorTotal.Should().Be(90m);
        resultado.Valor.Status.Should().Be("EM_DIAGNOSTICO");

        estoque.ObterItem(pecaInsumoCatalogo.Id).QuantidadeDisponivel.Should().Be(8);
        estoque.ObterItem(pecaInsumoCatalogo.Id).QuantidadeReservada.Should().Be(2);

        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.Is<OrdemServico>(ordemServicoAtualizada =>
            ordemServicoAtualizada.PecasInsumos.Single().PecaInsumoCatalogoId == pecaInsumoCatalogo.Id
            && ordemServicoAtualizada.ValorTotal == 90m), It.IsAny<CancellationToken>()), Times.Once);

        estoqueRepository.Verify(repo => repo.AtualizarAsync(estoque, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_OrdemServicoInexistente_Quando_ReservarPecaInsumo_Entao_DeveRetornarFalha()
    {
        // Arrange
        var pecaInsumoCatalogo = PecaInsumoCatalogoTestDataFactory.CriarPecaInsumoCatalogoPadrao();
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var pecaInsumoCatalogoRepository = new Mock<IPecaInsumoCatalogoRepository>();
        var estoqueRepository = new Mock<IEstoqueRepository>();
        var useCase = CriarUseCase(ordemServicoRepository, pecaInsumoCatalogoRepository, estoqueRepository);
        var request = new ReservarPecaInsumoRequest(
            Guid.NewGuid(),
            new[] { new PecaInsumoRequest(pecaInsumoCatalogo.Id, 1) });

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(OrdemServicoErrorMessages.OrdemServicoNaoEncontrada);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
        estoqueRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<Estoque>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_PecaInsumoCatalogoInexistente_Quando_ReservarPecaInsumo_Entao_DeveRetornarFalhaSemAlterarOrdemServico()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnostico();
        var estoque = EstoqueTestDataFactory.CriarEstoqueComItem(Guid.NewGuid());
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var pecaInsumoCatalogoRepository = new Mock<IPecaInsumoCatalogoRepository>();
        var estoqueRepository = new Mock<IEstoqueRepository>();

        ordemServicoRepository
            .Setup(repo => repo.ObterPorIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        estoqueRepository
            .Setup(repo => repo.ObterAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(estoque);

        var useCase = CriarUseCase(ordemServicoRepository, pecaInsumoCatalogoRepository, estoqueRepository);
        var request = new ReservarPecaInsumoRequest(
            ordemServico.Id,
            new[] { new PecaInsumoRequest(Guid.NewGuid(), 1) });

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(PecaInsumoCatalogoErrorMessages.PecaInsumoCatalogoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);
        ordemServico.PecasInsumos.Should().BeEmpty();

        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
        estoqueRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<Estoque>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_EstoqueInsuficiente_Quando_ReservarPecaInsumo_Entao_DeveRetornarFalhaSemAlterarOrdemServico()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnostico();
        var pecaInsumoCatalogo = PecaInsumoCatalogoTestDataFactory.CriarPecaInsumoCatalogoPadrao();
        var estoque = EstoqueTestDataFactory.CriarEstoqueComItem(pecaInsumoCatalogo.Id, quantidadeDisponivel: 1);
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var pecaInsumoCatalogoRepository = new Mock<IPecaInsumoCatalogoRepository>();
        var estoqueRepository = new Mock<IEstoqueRepository>();

        ordemServicoRepository
            .Setup(repo => repo.ObterPorIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        ConfigurarPecaInsumoCatalogo(pecaInsumoCatalogoRepository, pecaInsumoCatalogo);

        estoqueRepository
            .Setup(repo => repo.ObterAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(estoque);

        var useCase = CriarUseCase(ordemServicoRepository, pecaInsumoCatalogoRepository, estoqueRepository);
        var request = new ReservarPecaInsumoRequest(
            ordemServico.Id,
            new[] { new PecaInsumoRequest(pecaInsumoCatalogo.Id, 2) });

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(EstoqueErrorMessages.EstoqueInsuficiente);
        resultado.Erro.Tipo.Should().Be(TipoErro.RegraNegocio);
        ordemServico.PecasInsumos.Should().BeEmpty();

        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
        estoqueRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<Estoque>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_ListaPecasInsumosVazia_Quando_ReservarPecaInsumo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var pecaInsumoCatalogoRepository = new Mock<IPecaInsumoCatalogoRepository>();
        var estoqueRepository = new Mock<IEstoqueRepository>();
        var useCase = CriarUseCase(ordemServicoRepository, pecaInsumoCatalogoRepository, estoqueRepository);
        var request = new ReservarPecaInsumoRequest(Guid.NewGuid(), Array.Empty<PecaInsumoRequest>());

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);
    }

    [Fact]
    public async Task Dado_PecasInsumosRepetidas_Quando_ReservarPecaInsumo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var pecaInsumoCatalogoRepository = new Mock<IPecaInsumoCatalogoRepository>();
        var estoqueRepository = new Mock<IEstoqueRepository>();
        var useCase = CriarUseCase(ordemServicoRepository, pecaInsumoCatalogoRepository, estoqueRepository);
        var request = new ReservarPecaInsumoRequest(
            Guid.NewGuid(),
            new[]
            {
                new PecaInsumoRequest(pecaInsumoCatalogoId, 1),
                new PecaInsumoRequest(pecaInsumoCatalogoId, 1)
            });

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static ReservarPecaInsumoUseCase CriarUseCase(
        Mock<IOrdemServicoRepository> ordemServicoRepository,
        Mock<IPecaInsumoCatalogoRepository> pecaInsumoCatalogoRepository,
        Mock<IEstoqueRepository> estoqueRepository)
    {
        return new ReservarPecaInsumoUseCase(
            ordemServicoRepository.Object,
            pecaInsumoCatalogoRepository.Object,
            estoqueRepository.Object,
            new ReservarPecaInsumoValidator(),
            MapperFactory.Criar());
    }

    private static void ConfigurarPecaInsumoCatalogo(
        Mock<IPecaInsumoCatalogoRepository> repository,
        PecaInsumoCatalogo pecaInsumoCatalogo)
    {
        repository
            .Setup(repo => repo.ObterPorIdAsync(pecaInsumoCatalogo.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(pecaInsumoCatalogo);
    }
}







