using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.NotificarDecisaoOrcamento;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Factories;
using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Enums;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;
using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.OrdemServicoUseCases.NotificarDecisaoOrcamento;

public class NotificarDecisaoOrcamentoUseCaseTests
{
    [Fact]
    public async Task Dado_OrdemServicoAguardandoAprovacao_Quando_NotificarAprovacao_Entao_DeveIniciarExecucao()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoAguardandoAprovacao();
        var ordemServicoRepository = CriarOrdemServicoRepository(ordemServico);
        var estoqueRepository = new Mock<IEstoqueRepository>();
        var unitOfWork = CriarUnitOfWork();
        var useCase = CriarUseCase(ordemServicoRepository, estoqueRepository, unitOfWork);
        var request = new NotificarDecisaoOrcamentoRequest(ordemServico.Id, DecisaoOrcamento.Aprovado);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Status.Should().Be(OrdemServicoTestDataFactory.StatusEmExecucao);

        ordemServicoRepository.Verify(repo => repo.ObterPorIdAsync(request.OrdemServicoId, It.IsAny<CancellationToken>()), Times.Once);

        ordemServicoRepository.Verify(
            repo => repo.AtualizarAsync(
                It.Is<OrdemServico>(ordemServicoAtualizada =>
                    ordemServicoAtualizada.Id == ordemServico.Id
                    && ordemServicoAtualizada.Status.ToString() == OrdemServicoTestDataFactory.StatusEmExecucao),
                It.IsAny<CancellationToken>()),
            Times.Once);

        estoqueRepository.Verify(repo => repo.ObterAsync(It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(uow => uow.ExecutarEmTransacaoAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_OrdemServicoAguardandoAprovacaoComPecaReservada_Quando_NotificarRecusa_Entao_DeveCancelarComMotivoReprovacaoOrcamentoEEstornarEstoque()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoAguardandoAprovacaoComPecaInsumoReservado(pecaInsumoCatalogoId);
        var estoque = EstoqueTestDataFactory.CriarEstoqueComItemReservado(pecaInsumoCatalogoId);
        var ordemServicoRepository = CriarOrdemServicoRepository(ordemServico);
        var estoqueRepository = CriarEstoqueRepository(estoque);
        var unitOfWork = CriarUnitOfWork();
        var useCase = CriarUseCase(ordemServicoRepository, estoqueRepository, unitOfWork);
        var request = new NotificarDecisaoOrcamentoRequest(ordemServico.Id, DecisaoOrcamento.Recusado);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Status.Should().Be(OrdemServicoTestDataFactory.StatusCancelada);

        ordemServico.MotivoCancelamento.Should().Be(MotivoCancelamentoOrdemServico.ReprovacaoOrcamento);
        estoque.ObterItem(pecaInsumoCatalogoId).QuantidadeReservada.Should().Be(0);
        estoque.ObterItem(pecaInsumoCatalogoId).QuantidadeDisponivel.Should().Be(10);

        ordemServicoRepository.Verify(repo => repo.ObterPorIdAsync(request.OrdemServicoId, It.IsAny<CancellationToken>()), Times.Once);
        estoqueRepository.Verify(repo => repo.ObterAsync(It.IsAny<CancellationToken>()), Times.Once);

        ordemServicoRepository.Verify(
            repo => repo.AtualizarAsync(
                It.Is<OrdemServico>(ordemServicoAtualizada =>
                    ordemServicoAtualizada.Id == ordemServico.Id
                    && ordemServicoAtualizada.Status.ToString() == OrdemServicoTestDataFactory.StatusCancelada
                    && ordemServicoAtualizada.MotivoCancelamento == MotivoCancelamentoOrdemServico.ReprovacaoOrcamento),
                It.IsAny<CancellationToken>()),
            Times.Once);

        estoqueRepository.Verify(repo => repo.AtualizarAsync(estoque, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(uow => uow.ExecutarEmTransacaoAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_OrdemServicoInexistente_Quando_NotificarDecisao_Entao_DeveRetornarFalha()
    {
        // Arrange
        var ordemServicoRepository = CriarOrdemServicoRepository(null);
        var estoqueRepository = new Mock<IEstoqueRepository>();
        var useCase = CriarUseCase(ordemServicoRepository, estoqueRepository);
        var request = new NotificarDecisaoOrcamentoRequest(Guid.NewGuid(), DecisaoOrcamento.Aprovado);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(OrdemServicoErrorMessages.OrdemServicoNaoEncontrada);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        ordemServicoRepository.Verify(repo => repo.ObterPorIdAsync(request.OrdemServicoId, It.IsAny<CancellationToken>()), Times.Once);
        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
        estoqueRepository.Verify(repo => repo.ObterAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_OrdemServicoEmDiagnostico_Quando_NotificarAprovacao_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnosticoComServico();
        var ordemServicoRepository = CriarOrdemServicoRepository(ordemServico);
        var estoqueRepository = new Mock<IEstoqueRepository>();
        var useCase = CriarUseCase(ordemServicoRepository, estoqueRepository);
        var request = new NotificarDecisaoOrcamentoRequest(ordemServico.Id, DecisaoOrcamento.Aprovado);

        // Act
        var acao = () => useCase.ExecuteAsync(request);

        // Assert
        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.TransicaoStatusInvalida);

        ordemServicoRepository.Verify(repo => repo.ObterPorIdAsync(request.OrdemServicoId, It.IsAny<CancellationToken>()), Times.Once);
        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
        estoqueRepository.Verify(repo => repo.ObterAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_DecisaoInvalida_Quando_NotificarDecisao_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var estoqueRepository = new Mock<IEstoqueRepository>();
        var useCase = CriarUseCase(ordemServicoRepository, estoqueRepository);
        var request = new NotificarDecisaoOrcamentoRequest(Guid.NewGuid(), (DecisaoOrcamento)99);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        ordemServicoRepository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
        estoqueRepository.Verify(repo => repo.ObterAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IOrdemServicoRepository> CriarOrdemServicoRepository(OrdemServico? ordemServico)
    {
        var repository = new Mock<IOrdemServicoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

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
            .Setup(uow => uow.ExecutarEmTransacaoAsync(It.IsAny<Func<CancellationToken, Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<CancellationToken, Task>, CancellationToken>((operacao, cancellationToken) => operacao(cancellationToken));

        return unitOfWork;
    }

    private static NotificarDecisaoOrcamentoUseCase CriarUseCase(Mock<IOrdemServicoRepository> ordemServicoRepository, Mock<IEstoqueRepository> estoqueRepository, Mock<IUnitOfWork>? unitOfWork = null)
    {
        unitOfWork ??= CriarUnitOfWork();

        return new NotificarDecisaoOrcamentoUseCase(ordemServicoRepository.Object, estoqueRepository.Object, unitOfWork.Object, new NotificarDecisaoOrcamentoValidator(), MapperFactory.Criar());
    }
}
