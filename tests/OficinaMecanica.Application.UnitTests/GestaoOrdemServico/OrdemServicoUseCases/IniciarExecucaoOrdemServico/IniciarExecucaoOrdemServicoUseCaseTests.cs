using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarExecucaoOrdemServico;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Factories;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;
using OficinaMecanica.Domain.Shared.Exceptions;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.OrdemServicoUseCases.IniciarExecucaoOrdemServico;

public class IniciarExecucaoOrdemServicoUseCaseTests
{
    [Fact]
    public async Task Dado_OrdemServicoAguardandoAprovacao_Quando_IniciarExecucao_Entao_DeveAtualizarStatusEPersistir()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoAguardandoAprovacao();

        var repository = CriarRepository(ordemServico);

        var useCase = CriarUseCase(repository);

        var request = OrdemServicoTestDataFactory.CriarIniciarExecucaoOrdemServicoRequestValido(
            ordemServico.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(ordemServico.Id);
        resultado.Valor.Status.Should().Be(OrdemServicoTestDataFactory.StatusEmExecucao);

        repository.Verify(
            repo => repo.ObterPorIdAsync(
                request.OrdemServicoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            repo => repo.AtualizarAsync(
                It.Is<OrdemServico>(ordemServicoAtualizada =>
                    ordemServicoAtualizada.Id == ordemServico.Id
                    && ordemServicoAtualizada.Status.ToString() == OrdemServicoTestDataFactory.StatusEmExecucao),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_OrdemServicoInexistente_Quando_IniciarExecucao_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request = OrdemServicoTestDataFactory.CriarIniciarExecucaoOrdemServicoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(OrdemServicoErrorMessages.OrdemServicoNaoEncontrada);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(
            repo => repo.ObterPorIdAsync(
                request.OrdemServicoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<OrdemServico>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_OrdemServicoEmDiagnostico_Quando_IniciarExecucao_Entao_DeveLancarDomainException()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoEmDiagnostico();

        var repository = CriarRepository(ordemServico);

        var useCase = CriarUseCase(repository);

        var request = OrdemServicoTestDataFactory.CriarIniciarExecucaoOrdemServicoRequestValido(
            ordemServico.Id);

        // Act
        var acao = () => useCase.ExecuteAsync(request);

        // Assert
        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(OrdemServicoErrorMessages.TransicaoStatusInvalida);

        repository.Verify(
            repo => repo.ObterPorIdAsync(
                request.OrdemServicoId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<OrdemServico>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_IniciarExecucao_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IOrdemServicoRepository>();

        var useCase = CriarUseCase(repository);

        var request = OrdemServicoTestDataFactory.CriarIniciarExecucaoOrdemServicoRequestValido(
            Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        repository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<OrdemServico>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Mock<IOrdemServicoRepository> CriarRepository(OrdemServico? ordemServico)
    {
        var repository = new Mock<IOrdemServicoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        return repository;
    }

    private static IniciarExecucaoOrdemServicoUseCase CriarUseCase(
        Mock<IOrdemServicoRepository> repository)
    {
        return new IniciarExecucaoOrdemServicoUseCase(
            repository.Object,
            new IniciarExecucaoOrdemServicoValidator(),
            MapperFactory.Criar());
    }
}