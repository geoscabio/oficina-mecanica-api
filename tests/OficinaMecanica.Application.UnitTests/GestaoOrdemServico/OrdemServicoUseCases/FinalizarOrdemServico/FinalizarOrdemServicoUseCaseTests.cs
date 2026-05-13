using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.FinalizarOrdemServico;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Builders;
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
        var repository = new Mock<IOrdemServicoRepository>();
        repository
            .Setup(repo => repo.ObterPorIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        var estoqueRepository = new Mock<IEstoqueRepository>();
        var useCase = CriarUseCase(repository, estoqueRepository);

        // Act
        var resultado = await useCase.ExecuteAsync(new FinalizarOrdemServicoRequest(ordemServico.Id));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(ordemServico.Id);
        resultado.Valor.Status.Should().Be("FINALIZADA");
        resultado.Valor.DataFim.Should().NotBeNull();

        repository.Verify(repo => repo.AtualizarAsync(It.Is<OrdemServico>(ordemServicoAtualizada =>
            ordemServicoAtualizada.Id == ordemServico.Id
            && ordemServicoAtualizada.Status.ToString() == "FINALIZADA"
            && ordemServicoAtualizada.DataFim.HasValue), It.IsAny<CancellationToken>()), Times.Once);

        estoqueRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<Estoque>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_OrdemServicoComPecaInsumoReservado_Quando_FinalizarOrdemServico_Entao_DeveBaixarEstoqueEPersistir()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmExecucaoComServicoFinalizadoEPecaInsumoReservado(
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
        var resultado = await useCase.ExecuteAsync(new FinalizarOrdemServicoRequest(ordemServico.Id));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor!.Status.Should().Be("FINALIZADA");
        estoque.ObterItem(pecaInsumoCatalogoId).QuantidadeReservada.Should().Be(0);
        estoque.ObterItem(pecaInsumoCatalogoId).QuantidadeDisponivel.Should().Be(8);

        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.Is<OrdemServico>(ordemServicoAtualizada =>
            ordemServicoAtualizada.Id == ordemServico.Id
            && ordemServicoAtualizada.Status.ToString() == "FINALIZADA"), It.IsAny<CancellationToken>()), Times.Once);
        estoqueRepository.Verify(repo => repo.AtualizarAsync(estoque, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_OrdemServicoComPecaInsumoReservadoSemEstoque_Quando_FinalizarOrdemServico_Entao_DeveRetornarFalha()
    {
        // Arrange
        var pecaInsumoCatalogoId = Guid.NewGuid();
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmExecucaoComServicoFinalizadoEPecaInsumoReservado(
            pecaInsumoCatalogoId);
        var ordemServicoRepository = new Mock<IOrdemServicoRepository>();
        var estoqueRepository = new Mock<IEstoqueRepository>();
        ordemServicoRepository
            .Setup(repo => repo.ObterPorIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        var useCase = CriarUseCase(ordemServicoRepository, estoqueRepository);

        // Act
        var resultado = await useCase.ExecuteAsync(new FinalizarOrdemServicoRequest(ordemServico.Id));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(EstoqueErrorMessages.EstoqueNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        ordemServicoRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
        estoqueRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<Estoque>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_OrdemServicoInexistente_Quando_FinalizarOrdemServico_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = new Mock<IOrdemServicoRepository>();
        var estoqueRepository = new Mock<IEstoqueRepository>();
        var useCase = CriarUseCase(repository, estoqueRepository);

        // Act
        var resultado = await useCase.ExecuteAsync(new FinalizarOrdemServicoRequest(Guid.NewGuid()));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(OrdemServicoErrorMessages.OrdemServicoNaoEncontrada);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
        estoqueRepository.Verify(repo => repo.ObterAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_OrdemServicoEmExecucaoComServicoPendente_Quando_FinalizarOrdemServico_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmExecucaoComServicoPendente();
        var repository = new Mock<IOrdemServicoRepository>();
        repository
            .Setup(repo => repo.ObterPorIdAsync(ordemServico.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        var estoqueRepository = new Mock<IEstoqueRepository>();
        var useCase = CriarUseCase(repository, estoqueRepository);

        // Act
        var acao = () => useCase.ExecuteAsync(new FinalizarOrdemServicoRequest(ordemServico.Id));

        // Assert
        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.ServicosFinalizadosObrigatorios);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
        estoqueRepository.Verify(repo => repo.AtualizarAsync(It.IsAny<Estoque>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_FinalizarOrdemServico_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IOrdemServicoRepository>();
        var estoqueRepository = new Mock<IEstoqueRepository>();
        var useCase = CriarUseCase(repository, estoqueRepository);

        // Act
        var resultado = await useCase.ExecuteAsync(new FinalizarOrdemServicoRequest(Guid.Empty));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static FinalizarOrdemServicoUseCase CriarUseCase(
        Mock<IOrdemServicoRepository> repository,
        Mock<IEstoqueRepository> estoqueRepository)
    {
        return new FinalizarOrdemServicoUseCase(
            repository.Object,
            estoqueRepository.Object,
            new FinalizarOrdemServicoValidator(),
            MapperFactory.Criar());
    }
}
