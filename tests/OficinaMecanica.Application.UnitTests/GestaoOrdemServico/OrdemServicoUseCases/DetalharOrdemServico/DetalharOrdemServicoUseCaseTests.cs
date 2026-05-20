using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.DetalharOrdemServico;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Factories;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.OrdemServicoUseCases.DetalharOrdemServico;

public class DetalharOrdemServicoUseCaseTests
{
    [Fact]
    public async Task Dado_OrdemServicoExistente_Quando_DetalharOrdemServico_Entao_DeveRetornarOrdemServico()
    {
        // Arrange
        var ordemServico = OrdemServicoTestDataFactory.CriarOrdemServicoRecebida();

        var repository = CriarRepository(ordemServico);

        var useCase = CriarUseCase(repository);

        var request = OrdemServicoTestDataFactory.CriarDetalharOrdemServicoRequestValido(ordemServico.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(ordemServico.Id);
        resultado.Valor.Numero.Should().Be(ordemServico.Numero);
        resultado.Valor.Status.Should().Be(OrdemServicoTestDataFactory.StatusRecebida);
        resultado.Valor.VeiculoId.Should().Be(ordemServico.VeiculoId);
        resultado.Valor.MecanicoId.Should().Be(ordemServico.MecanicoId);

        repository.Verify(repo => repo.ObterPorIdAsync(request.OrdemServicoId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_OrdemServicoInexistente_Quando_DetalharOrdemServico_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request = OrdemServicoTestDataFactory.CriarDetalharOrdemServicoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(OrdemServicoErrorMessages.OrdemServicoNaoEncontrada);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(repo => repo.ObterPorIdAsync(request.OrdemServicoId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_DetalharOrdemServico_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IOrdemServicoRepository>();

        var useCase = CriarUseCase(repository);

        var request = OrdemServicoTestDataFactory.CriarDetalharOrdemServicoRequestValido(Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IOrdemServicoRepository> CriarRepository(OrdemServico? ordemServico)
    {
        var repository = new Mock<IOrdemServicoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ordemServico);

        return repository;
    }

    private static DetalharOrdemServicoUseCase CriarUseCase(Mock<IOrdemServicoRepository> repository)
    {
        return new DetalharOrdemServicoUseCase(repository.Object, new DetalharOrdemServicoValidator(), MapperFactory.Criar());
    }
}