using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.CancelarOrdemServico;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Builders;
using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;
using OficinaMecanica.Domain.GestaoEstoque.Messages;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Enums;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;
using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.OrdemServicoUseCases.CancelarOrdemServico;

public class CancelarOrdemServicoUseCaseTests
{
    [Fact]
    public async Task Dado_OrdemServicoAguardandoAprovacaoComPecaReservada_Quando_CancelarPorReprovacao_Entao_DeveEstornarEstoqueEPersistir()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoAguardandoAprovacaoComPecaInsumoReservado(
            pecaInsumoCatalogoId);
        var estoque = EstoqueTestDataFactory.CriarEstoqueComItemReservado(pecaInsumoCatalogoId);
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var estoqueRepository = new Mock<IEstoqueRepository>();
        ordemServicoRepository
            .Setup(repo => repo.ObterPorIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);
        estoqueRepository
            .Setup(repo => repo.ObterAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(estoque);

        var useCase = CriarUseCase(ordemServicoRepository, estoqueRepository);

        // Act
        var resultado = await useCase.ExecuteAsync(new CancelarOrdemServicoRequest(
            ordemServico.Id,
            MotivoCancelamentoOrdemServico.ReprovacaoOrcamento));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor!.Status.Should().Be("CANCELADA");
        ordemServico.MotivoCancelamento.Should().Be(MotivoCancelamentoOrdemServico.ReprovacaoOrcamento);
        estoque.ObterItem(pecaInsumoCatalogoId).QuantidadeReservada.Should().Be(0);
        estoque.ObterItem(pecaInsumoCatalogoId).QuantidadeDisponivel.Should().Be(10);

        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.Is<OrdemServico>(ordemServicoAtualizada =>
            ordemServicoAtualizada.Id == ordemServico.Id
            && ordemServicoAtualizada.Status.ToString() == "CANCELADA"), It.IsAny<CancellationToken>()), Times.Once);
        estoqueRepository.Verify(repo => repo.AtualizarAsync(estoque, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_OrdemServicoAguardandoAprovacaoComPecaReservada_Quando_CancelarPorOutroMotivo_Entao_NaoDeveEstornarEstoque()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoAguardandoAprovacaoComPecaInsumoReservado(
            pecaInsumoCatalogoId);
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var estoqueRepository = new Mock<IEstoqueRepository>();
        ordemServicoRepository
            .Setup(repo => repo.ObterPorIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        var useCase = CriarUseCase(ordemServicoRepository, estoqueRepository);

        // Act
        var resultado = await useCase.ExecuteAsync(new CancelarOrdemServicoRequest(
            ordemServico.Id,
            MotivoCancelamentoOrdemServico.EstoqueInsuficiente));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor!.Status.Should().Be("CANCELADA");
        ordemServico.MotivoCancelamento.Should().Be(MotivoCancelamentoOrdemServico.EstoqueInsuficiente);

        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Once);
        estoqueRepository.Verify(repo => repo.ObterAsync(It.IsAny<CancellationToken>()), Times.Never);
        estoqueRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<Estoque>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_OrdemServicoComPecaReservadaSemEstoque_Quando_CancelarPorReprovacao_Entao_DeveRetornarFalha()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoAguardandoAprovacaoComPecaInsumoReservado(
            pecaInsumoCatalogoId);
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var estoqueRepository = new Mock<IEstoqueRepository>();
        ordemServicoRepository
            .Setup(repo => repo.ObterPorIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        var useCase = CriarUseCase(ordemServicoRepository, estoqueRepository);

        // Act
        var resultado = await useCase.ExecuteAsync(new CancelarOrdemServicoRequest(
            ordemServico.Id,
            MotivoCancelamentoOrdemServico.ReprovacaoOrcamento));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(EstoqueErrorMessages.EstoqueNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
        estoqueRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<Estoque>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_OrdemServicoInexistente_Quando_Cancelar_Entao_DeveRetornarFalha()
    {
        // Arrange
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var estoqueRepository = new Mock<IEstoqueRepository>();
        var useCase = CriarUseCase(ordemServicoRepository, estoqueRepository);

        // Act
        var resultado = await useCase.ExecuteAsync(new CancelarOrdemServicoRequest(
            Guid.NewGuid(),
            MotivoCancelamentoOrdemServico.ReprovacaoOrcamento));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(OrdemServicoErrorMessages.OrdemServicoNaoEncontrada);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        estoqueRepository.Verify(repo => repo.ObterAsync(It.IsAny<CancellationToken>()), Times.Never);
        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_OrdemServicoFinalizada_Quando_Cancelar_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmExecucaoComServicoFinalizado();
        ordemServico.Finalizar();
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var estoqueRepository = new Mock<IEstoqueRepository>();
        ordemServicoRepository
            .Setup(repo => repo.ObterPorIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        var useCase = CriarUseCase(ordemServicoRepository, estoqueRepository);

        // Act
        var acao = () => useCase.ExecuteAsync(new CancelarOrdemServicoRequest(
            ordemServico.Id,
            MotivoCancelamentoOrdemServico.EstoqueInsuficiente));

        // Assert
        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.CancelamentoStatusInvalido);

        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
        estoqueRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<Estoque>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_MotivoInvalido_Quando_Cancelar_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var estoqueRepository = new Mock<IEstoqueRepository>();
        var useCase = CriarUseCase(ordemServicoRepository, estoqueRepository);

        // Act
        var resultado = await useCase.ExecuteAsync(new CancelarOrdemServicoRequest(
            Guid.NewGuid(),
            (MotivoCancelamentoOrdemServico)99));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static CancelarOrdemServicoUseCase CriarUseCase(
        Mock<IOrdemServicoRepository> ordemServicoRepository,
        Mock<IEstoqueRepository> estoqueRepository)
    {
        return new CancelarOrdemServicoUseCase(
            ordemServicoRepository.Object,
            estoqueRepository.Object,
            new CancelarOrdemServicoValidator(),
            MapperFactory.Criar());
    }
}
