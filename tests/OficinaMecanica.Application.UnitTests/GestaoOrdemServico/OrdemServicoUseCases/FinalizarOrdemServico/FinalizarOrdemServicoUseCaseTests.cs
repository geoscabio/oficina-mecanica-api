using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.FinalizarOrdemServico;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Factories;
using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;
using OficinaMecanica.Domain.GestaoEstoque.Messages;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;
using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.OrdemServicoUseCases.FinalizarOrdemServico;

public class FinalizarOrdemServicoUseCaseTests
{
    [Fact]
    public async Task Dado_OrdemServicoEmExecucaoComServicosFinalizados_Quando_FinalizarOrdemServico_Entao_DeveAtualizarStatusEPersistir()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmExecucaoComServicoFinalizado();

        var ordemServicoRepository = CriarOrdemServicoRepository(ordemServico);

        var estoqueRepository = CriarEstoqueRepository(null);

        var useCase = CriarUseCase(
            ordemServicoRepository,
            estoqueRepository);

        var request = OrdemServicoTestDataFactory.CriarFinalizarOrdemServicoRequestValido(
            ordemServico.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(ordemServico.Id);
        resultado.Valor.Status.Should().Be(OrdemServicoTestDataFactory.StatusFinalizada);
        resultado.Valor.DataFim.Should().NotBeNull();

        ordemServicoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                request.OrdemServicoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        estoqueRepository.Verify(
            repo => repo.ObterAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        ordemServicoRepository.Verify(
            repo => repo.AtualizarAsync(
                It.Is<OrdemServico>(ordemServicoAtualizada =>
                    ordemServicoAtualizada.Id == ordemServico.Id
                    && ordemServicoAtualizada.Status.ToString() == OrdemServicoTestDataFactory.StatusFinalizada
                    && ordemServicoAtualizada.DataFim.HasValue),
                It.IsAny<CancellationToken>()),
            Times.Once);

        estoqueRepository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<Estoque>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_OrdemServicoComPecaInsumoReservado_Quando_FinalizarOrdemServico_Entao_DeveBaixarEstoqueEPersistir()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();

        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmExecucaoComServicoFinalizadoEPecaInsumoReservado(
            pecaInsumoCatalogoId);

        var estoque = EstoqueTestDataFactory.CriarEstoqueComItemReservado(
            pecaInsumoCatalogoId);

        var ordemServicoRepository = CriarOrdemServicoRepository(ordemServico);

        var estoqueRepository = CriarEstoqueRepository(estoque);

        var useCase = CriarUseCase(
            ordemServicoRepository,
            estoqueRepository);

        var request = OrdemServicoTestDataFactory.CriarFinalizarOrdemServicoRequestValido(
            ordemServico.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Status.Should().Be(OrdemServicoTestDataFactory.StatusFinalizada);

        estoque.ObterItem(pecaInsumoCatalogoId).QuantidadeReservada.Should().Be(0);
        estoque.ObterItem(pecaInsumoCatalogoId).QuantidadeDisponivel.Should().Be(8);

        ordemServicoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                request.OrdemServicoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        estoqueRepository.Verify(
            repo => repo.ObterAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        ordemServicoRepository.Verify(
            repo => repo.AtualizarAsync(
                It.Is<OrdemServico>(ordemServicoAtualizada =>
                    ordemServicoAtualizada.Id == ordemServico.Id
                    && ordemServicoAtualizada.Status.ToString() == OrdemServicoTestDataFactory.StatusFinalizada),
                It.IsAny<CancellationToken>()),
            Times.Once);

        estoqueRepository.Verify(
            repo => repo.AtualizarAsync(
                estoque,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_OrdemServicoComPecaInsumoReservadoSemEstoque_Quando_FinalizarOrdemServico_Entao_DeveRetornarFalha()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();

        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmExecucaoComServicoFinalizadoEPecaInsumoReservado(
            pecaInsumoCatalogoId);

        var ordemServicoRepository = CriarOrdemServicoRepository(ordemServico);

        var estoqueRepository = CriarEstoqueRepository(null);

        var useCase = CriarUseCase(
            ordemServicoRepository,
            estoqueRepository);

        var request = OrdemServicoTestDataFactory.CriarFinalizarOrdemServicoRequestValido(
            ordemServico.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(EstoqueErrorMessages.EstoqueNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        ordemServicoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                request.OrdemServicoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        estoqueRepository.Verify(
            repo => repo.ObterAsync(It.IsAny<CancellationToken>()),
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
    public async Task Dado_OrdemServicoInexistente_Quando_FinalizarOrdemServico_Entao_DeveRetornarFalha()
    {
        // Arrange
        var ordemServicoRepository = CriarOrdemServicoRepository(null);

        var estoqueRepository = new Mock<IEstoqueRepository>();

        var useCase = CriarUseCase(
            ordemServicoRepository,
            estoqueRepository);

        var request = OrdemServicoTestDataFactory.CriarFinalizarOrdemServicoRequestValido();

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
    public async Task Dado_OrdemServicoEmExecucaoComServicoPendente_Quando_FinalizarOrdemServico_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmExecucaoComServicoPendente();

        var ordemServicoRepository = CriarOrdemServicoRepository(ordemServico);

        var estoqueRepository = CriarEstoqueRepository(null);

        var useCase = CriarUseCase(
            ordemServicoRepository,
            estoqueRepository);

        var request = OrdemServicoTestDataFactory.CriarFinalizarOrdemServicoRequestValido(
            ordemServico.Id);

        // Act
        var acao = () => useCase.ExecuteAsync(request);

        // Assert
        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.ServicosFinalizadosObrigatorios);

        ordemServicoRepository.Verify(
            repo => repo.ObterPorIdAsync(
                request.OrdemServicoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        estoqueRepository.Verify(
            repo => repo.ObterAsync(It.IsAny<CancellationToken>()),
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
    public async Task Dado_IdVazio_Quando_FinalizarOrdemServico_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();

        var estoqueRepository = new Mock<IEstoqueRepository>();

        var useCase = CriarUseCase(
            ordemServicoRepository,
            estoqueRepository);

        var request = OrdemServicoTestDataFactory.CriarFinalizarOrdemServicoRequestValido(
            Guid.Empty);

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

    private static Mock<IEstoqueRepository> CriarEstoqueRepository(Estoque? estoque)
    {
        var repository = new Mock<IEstoqueRepository>();

        repository
            .Setup(repo => repo.ObterAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(estoque);

        return repository;
    }

    private static FinalizarOrdemServicoUseCase CriarUseCase(
        Mock<IOrdemServicoRepository> ordemServicoRepository,
        Mock<IEstoqueRepository> estoqueRepository)
    {
        return new FinalizarOrdemServicoUseCase(
            ordemServicoRepository.Object,
            estoqueRepository.Object,
            new FinalizarOrdemServicoValidator(),
            MapperFactory.Criar());
    }
}
