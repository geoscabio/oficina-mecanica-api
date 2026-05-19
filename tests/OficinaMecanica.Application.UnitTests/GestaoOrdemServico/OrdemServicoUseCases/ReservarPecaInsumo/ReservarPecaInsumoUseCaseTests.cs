using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ReservarPecaInsumo;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Factories;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;
using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;
using OficinaMecanica.Domain.GestaoEstoque.Messages;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;

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

        var ordemServicoRepository = CriarOrdemServicoRepository(ordemServico);

        var pecaInsumoCatalogoRepository = CriarPecaInsumoCatalogoRepository(pecaInsumoCatalogo);

        var estoqueRepository = CriarEstoqueRepository(estoque);

        var unitOfWork = CriarUnitOfWork();

        var useCase = CriarUseCase(
            ordemServicoRepository,
            pecaInsumoCatalogoRepository,
            estoqueRepository,
            unitOfWork);

        var request = OrdemServicoTestDataFactory.CriarReservarPecaInsumoRequestValido(
            ordemServico.Id,
            pecaInsumoCatalogo.Id,
            OrdemServicoTestDataFactory.QuantidadePecaInsumoPadrao);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(ordemServico.Id);
        resultado.Valor.ValorTotal.Should().Be(90m);
        resultado.Valor.Status.Should().Be(OrdemServicoTestDataFactory.StatusEmDiagnostico);

        estoque.ObterItem(pecaInsumoCatalogo.Id).QuantidadeDisponivel.Should().Be(8);
        estoque.ObterItem(pecaInsumoCatalogo.Id).QuantidadeReservada.Should().Be(2);

        ordemServicoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                request.OrdemServicoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        estoqueRepository.Verify(
            repo => repo.ObterAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        pecaInsumoCatalogoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                pecaInsumoCatalogo.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);

        ordemServicoRepository.Verify(
            repo => repo.AtualizarAsync(
                It.Is<OrdemServico>(ordemServicoAtualizada =>
                    ordemServicoAtualizada.PecasInsumos.Single().PecaInsumoCatalogoId == pecaInsumoCatalogo.Id
                    && ordemServicoAtualizada.PecasInsumos.Single().Quantidade == OrdemServicoTestDataFactory.QuantidadePecaInsumoPadrao
                    && ordemServicoAtualizada.ValorTotal == 90m),
                It.IsAny<CancellationToken>()),
            Times.Once);

        estoqueRepository.Verify(
            repo => repo.AtualizarAsync(
                estoque,
                It.IsAny<CancellationToken>()),
            Times.Once);

        unitOfWork.Verify(
            uow => uow.ExecutarEmTransacaoAsync(
                It.IsAny<Func<CancellationToken, Task>>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_OrdemServicoInexistente_Quando_ReservarPecaInsumo_Entao_DeveRetornarFalha()
    {
        // Arrange
        var pecaInsumoCatalogo = PecaInsumoCatalogoTestDataFactory.CriarPecaInsumoCatalogoPadrao();

        var ordemServicoRepository = CriarOrdemServicoRepository(null);

        var pecaInsumoCatalogoRepository = new Mock<IPecaInsumoCatalogoRepository>();

        var estoqueRepository = new Mock<IEstoqueRepository>();

        var useCase = CriarUseCase(
            ordemServicoRepository,
            pecaInsumoCatalogoRepository,
            estoqueRepository);

        var request = OrdemServicoTestDataFactory.CriarReservarPecaInsumoRequestValido(
            pecaInsumoCatalogoId: pecaInsumoCatalogo.Id,
            quantidade: 1);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(OrdemServicoErrorMessages.OrdemServicoNaoEncontrada);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        ordemServicoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                request.OrdemServicoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        estoqueRepository.Verify(
            repo => repo.ObterAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        pecaInsumoCatalogoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        ordemServicoRepository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<OrdemServico>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        estoqueRepository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<Estoque>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_EstoqueInexistente_Quando_ReservarPecaInsumo_Entao_DeveRetornarFalhaSemAlterarOrdemServico()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnostico();

        var pecaInsumoCatalogo = PecaInsumoCatalogoTestDataFactory.CriarPecaInsumoCatalogoPadrao();

        var ordemServicoRepository = CriarOrdemServicoRepository(ordemServico);

        var pecaInsumoCatalogoRepository = new Mock<IPecaInsumoCatalogoRepository>();

        var estoqueRepository = CriarEstoqueRepository(null);

        var useCase = CriarUseCase(
            ordemServicoRepository,
            pecaInsumoCatalogoRepository,
            estoqueRepository);

        var request = OrdemServicoTestDataFactory.CriarReservarPecaInsumoRequestValido(
            ordemServico.Id,
            pecaInsumoCatalogo.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(EstoqueErrorMessages.EstoqueNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        ordemServico.PecasInsumos.Should().BeEmpty();

        ordemServicoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                request.OrdemServicoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        estoqueRepository.Verify(
            repo => repo.ObterAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        pecaInsumoCatalogoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        ordemServicoRepository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<OrdemServico>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        estoqueRepository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<Estoque>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_PecaInsumoCatalogoInexistente_Quando_ReservarPecaInsumo_Entao_DeveRetornarFalhaSemAlterarOrdemServico()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnostico();

        var pecaInsumoCatalogoId = Guid.NewGuid();

        var estoque = EstoqueTestDataFactory.CriarEstoqueComItem(pecaInsumoCatalogoId);

        var ordemServicoRepository = CriarOrdemServicoRepository(ordemServico);

        var pecaInsumoCatalogoRepository = new Mock<IPecaInsumoCatalogoRepository>();

        var estoqueRepository = CriarEstoqueRepository(estoque);

        var useCase = CriarUseCase(
            ordemServicoRepository,
            pecaInsumoCatalogoRepository,
            estoqueRepository);

        var request = OrdemServicoTestDataFactory.CriarReservarPecaInsumoRequestValido(
            ordemServico.Id,
            pecaInsumoCatalogoId,
            quantidade: 1);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(PecaInsumoCatalogoErrorMessages.PecaInsumoCatalogoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        ordemServico.PecasInsumos.Should().BeEmpty();

        ordemServicoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                request.OrdemServicoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        estoqueRepository.Verify(
            repo => repo.ObterAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        pecaInsumoCatalogoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                pecaInsumoCatalogoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        ordemServicoRepository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<OrdemServico>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        estoqueRepository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<Estoque>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_EstoqueInsuficiente_Quando_ReservarPecaInsumo_Entao_DeveRetornarFalhaSemAlterarOrdemServico()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnostico();

        var pecaInsumoCatalogo = PecaInsumoCatalogoTestDataFactory.CriarPecaInsumoCatalogoPadrao();

        var estoque = EstoqueTestDataFactory.CriarEstoqueComItem(
            pecaInsumoCatalogo.Id,
            quantidadeDisponivel: 1);

        var ordemServicoRepository = CriarOrdemServicoRepository(ordemServico);

        var pecaInsumoCatalogoRepository = CriarPecaInsumoCatalogoRepository(pecaInsumoCatalogo);

        var estoqueRepository = CriarEstoqueRepository(estoque);

        var useCase = CriarUseCase(
            ordemServicoRepository,
            pecaInsumoCatalogoRepository,
            estoqueRepository);

        var request = OrdemServicoTestDataFactory.CriarReservarPecaInsumoRequestValido(
            ordemServico.Id,
            pecaInsumoCatalogo.Id,
            quantidade: 2);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(EstoqueErrorMessages.EstoqueInsuficiente);
        resultado.Erro.Tipo.Should().Be(TipoErro.RegraNegocio);

        ordemServico.PecasInsumos.Should().BeEmpty();

        ordemServicoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                request.OrdemServicoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        estoqueRepository.Verify(
            repo => repo.ObterAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        pecaInsumoCatalogoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                pecaInsumoCatalogo.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);

        ordemServicoRepository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<OrdemServico>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        estoqueRepository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<Estoque>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_ReservarPecaInsumo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();

        var pecaInsumoCatalogoRepository = new Mock<IPecaInsumoCatalogoRepository>();

        var estoqueRepository = new Mock<IEstoqueRepository>();

        var useCase = CriarUseCase(
            ordemServicoRepository,
            pecaInsumoCatalogoRepository,
            estoqueRepository);

        var request = OrdemServicoTestDataFactory.CriarReservarPecaInsumoRequestValido(
            ordemServicoId: Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        ordemServicoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        estoqueRepository.Verify(
            repo => repo.ObterAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        pecaInsumoCatalogoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        ordemServicoRepository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<OrdemServico>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        estoqueRepository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<Estoque>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_ListaPecasInsumosVazia_Quando_ReservarPecaInsumo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();

        var pecaInsumoCatalogoRepository = new Mock<IPecaInsumoCatalogoRepository>();

        var estoqueRepository = new Mock<IEstoqueRepository>();

        var useCase = CriarUseCase(
            ordemServicoRepository,
            pecaInsumoCatalogoRepository,
            estoqueRepository);

        var request = new ReservarPecaInsumoRequest(
            Guid.NewGuid(),
            Array.Empty<PecaInsumoRequest>());

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        ordemServicoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        estoqueRepository.Verify(
            repo => repo.ObterAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        pecaInsumoCatalogoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_PecaInsumoCatalogoIdVazio_Quando_ReservarPecaInsumo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();

        var pecaInsumoCatalogoRepository = new Mock<IPecaInsumoCatalogoRepository>();

        var estoqueRepository = new Mock<IEstoqueRepository>();

        var useCase = CriarUseCase(
            ordemServicoRepository,
            pecaInsumoCatalogoRepository,
            estoqueRepository);

        var request = OrdemServicoTestDataFactory.CriarReservarPecaInsumoRequestValido(
            pecaInsumoCatalogoId: Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        ordemServicoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        estoqueRepository.Verify(
            repo => repo.ObterAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        pecaInsumoCatalogoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_QuantidadeInvalida_Quando_ReservarPecaInsumo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();

        var pecaInsumoCatalogoRepository = new Mock<IPecaInsumoCatalogoRepository>();

        var estoqueRepository = new Mock<IEstoqueRepository>();

        var useCase = CriarUseCase(
            ordemServicoRepository,
            pecaInsumoCatalogoRepository,
            estoqueRepository);

        var request = OrdemServicoTestDataFactory.CriarReservarPecaInsumoRequestValido(
            quantidade: 0);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        ordemServicoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        estoqueRepository.Verify(
            repo => repo.ObterAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        pecaInsumoCatalogoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_PecasInsumosRepetidas_Quando_ReservarPecaInsumo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();

        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();

        var pecaInsumoCatalogoRepository = new Mock<IPecaInsumoCatalogoRepository>();

        var estoqueRepository = new Mock<IEstoqueRepository>();

        var useCase = CriarUseCase(
            ordemServicoRepository,
            pecaInsumoCatalogoRepository,
            estoqueRepository);

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

        ordemServicoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        estoqueRepository.Verify(
            repo => repo.ObterAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        pecaInsumoCatalogoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Mock<IOrdemServicoRepository> CriarOrdemServicoRepository(
        OrdemServico? ordemServico)
    {
        var repository = new Mock<IOrdemServicoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        return repository;
    }

    private static Mock<IPecaInsumoCatalogoRepository> CriarPecaInsumoCatalogoRepository(
        params PecaInsumoCatalogo[] pecasInsumosCatalogo)
    {
        var repository = new Mock<IPecaInsumoCatalogoRepository>();

        foreach (var pecaInsumoCatalogo in pecasInsumosCatalogo)
        {
            repository
                .Setup(repo => repo.ObterPorIdAsync(
                    pecaInsumoCatalogo.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pecaInsumoCatalogo);
        }

        return repository;
    }

    private static Mock<IEstoqueRepository> CriarEstoqueRepository(Estoque? estoque)
    {
        var repository = new Mock<IEstoqueRepository>();

        repository
            .Setup(repo => repo.ObterAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(estoque);

        return repository;
    }

    private static Mock<IUnitOfWork> CriarUnitOfWork()
    {
        var unitOfWork = new Mock<IUnitOfWork>();

        unitOfWork
            .Setup(uow => uow.ExecutarEmTransacaoAsync(
                It.IsAny<Func<CancellationToken, Task>>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, CancellationToken>(
                (operacao, cancellationToken) => operacao(cancellationToken));

        return unitOfWork;
    }

    private static ReservarPecaInsumoUseCase CriarUseCase(
        Mock<IOrdemServicoRepository> ordemServicoRepository,
        Mock<IPecaInsumoCatalogoRepository> pecaInsumoCatalogoRepository,
        Mock<IEstoqueRepository> estoqueRepository,
        Mock<IUnitOfWork>? unitOfWork = null)
    {
        unitOfWork ??= CriarUnitOfWork();

        return new ReservarPecaInsumoUseCase(
            ordemServicoRepository.Object,
            pecaInsumoCatalogoRepository.Object,
            estoqueRepository.Object,
            unitOfWork.Object,
            new ReservarPecaInsumoValidator(),
            MapperFactory.Criar());
    }
}
