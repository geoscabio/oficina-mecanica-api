using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarExecucaoServico;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Factories;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Enums;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;
using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.OrdemServicoUseCases.IniciarExecucaoServico;

public class IniciarExecucaoServicoUseCaseTests
{
    [Fact]
    public async Task Dado_OrdemServicoEmExecucaoComServicoPendente_Quando_IniciarExecucaoServico_Entao_DeveAtualizarServicoEPersistir()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmExecucaoComServicoPendente();

        var servicoId = ordemServico.Servicos.Single().Id;

        var repository = CriarRepository(ordemServico);

        var useCase = CriarUseCase(repository);

        var request = OrdemServicoTestDataFactory.CriarIniciarExecucaoServicoRequestValido(ordemServico.Id, servicoId);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(ordemServico.Id);
        ordemServico.Servicos.Single().Status.Should().Be(StatusServico.EmExecucao);

        repository.Verify(repo => repo.ObterPorIdAsync(request.OrdemServicoId, It.IsAny<CancellationToken>()), Times.Once);

        repository.Verify(
            repo => repo.AtualizarAsync(
                It.Is<OrdemServico>(ordemServicoAtualizada =>
                    ordemServicoAtualizada.Id == ordemServico.Id
                    && ordemServicoAtualizada.Servicos.Single().Id == servicoId
                    && ordemServicoAtualizada.Servicos.Single().Status == StatusServico.EmExecucao),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_OrdemServicoInexistente_Quando_IniciarExecucaoServico_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request = OrdemServicoTestDataFactory.CriarIniciarExecucaoServicoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(OrdemServicoErrorMessages.OrdemServicoNaoEncontrada);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(repo => repo.ObterPorIdAsync(request.OrdemServicoId, It.IsAny<CancellationToken>()), Times.Once);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_ServicoInexistente_Quando_IniciarExecucaoServico_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmExecucaoComServicoPendente();

        var repository = CriarRepository(ordemServico);

        var useCase = CriarUseCase(repository);

        var request = OrdemServicoTestDataFactory.CriarIniciarExecucaoServicoRequestValido(ordemServico.Id, Guid.NewGuid());

        // Act
        var acao = () => useCase.ExecuteAsync(request);

        // Assert
        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.ServicoNaoEncontrado);

        repository.Verify(repo => repo.ObterPorIdAsync(request.OrdemServicoId, It.IsAny<CancellationToken>()), Times.Once);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_OrdemServicoIdVazio_Quando_IniciarExecucaoServico_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IOrdemServicoRepository>();

        var useCase = CriarUseCase(repository);

        var request = OrdemServicoTestDataFactory.CriarIniciarExecucaoServicoRequestValido(ordemServicoId: Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_ServicoIdVazio_Quando_IniciarExecucaoServico_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IOrdemServicoRepository>();

        var useCase = CriarUseCase(repository);

        var request = OrdemServicoTestDataFactory.CriarIniciarExecucaoServicoRequestValido(servicoId: Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.AtualizarAsync(It.IsAny<OrdemServico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IOrdemServicoRepository> CriarRepository(OrdemServico? ordemServico)
    {
        var repository = new Mock<IOrdemServicoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        return repository;
    }

    private static IniciarExecucaoServicoUseCase CriarUseCase(Mock<IOrdemServicoRepository> repository)
    {
        return new IniciarExecucaoServicoUseCase(repository.Object, new IniciarExecucaoServicoValidator(), MapperFactory.Criar());
    }
}
