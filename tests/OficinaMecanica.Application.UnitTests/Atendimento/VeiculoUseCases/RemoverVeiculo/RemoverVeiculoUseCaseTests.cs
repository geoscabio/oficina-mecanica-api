using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.RemoverVeiculo;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Atendimento.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.Messages;

namespace OficinaMecanica.Application.UnitTests.Atendimento.VeiculoUseCases.RemoverVeiculo;

public class RemoverVeiculoUseCaseTests
{
    [Fact]
    public async Task Dado_VeiculoExistente_Quando_RemoverVeiculo_Entao_DeveRemoverVeiculo()
    {
        // Arrange
        var veiculo = VeiculoTestDataFactory.CriarVeiculoPadrao();

        var repository = CriarRepository(veiculo);

        var useCase = CriarUseCase(repository);

        var request = VeiculoTestDataFactory.CriarRemoverVeiculoRequestValido(veiculo.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(veiculo.Id);

        repository.Verify(repo => repo.ObterPorIdAsync(request.VeiculoId, It.IsAny<CancellationToken>()), Times.Once);

        repository.Verify(repo => repo.RemoverAsync(It.Is<Veiculo>(veiculoRemovido => veiculoRemovido.Id == veiculo.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_VeiculoInexistente_Quando_RemoverVeiculo_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request = VeiculoTestDataFactory.CriarRemoverVeiculoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(VeiculoErrorMessages.VeiculoNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(repo => repo.ObterPorIdAsync(request.VeiculoId, It.IsAny<CancellationToken>()), Times.Once);

        repository.Verify(repo => repo.RemoverAsync(It.IsAny<Veiculo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_RemoverVeiculo_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IVeiculoRepository>();

        var useCase = CriarUseCase(repository);

        var request = VeiculoTestDataFactory.CriarRemoverVeiculoRequestValido(Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

        repository.Verify(repo => repo.RemoverAsync(It.IsAny<Veiculo>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<IVeiculoRepository> CriarRepository(Veiculo? veiculo)
    {
        var repository = new Mock<IVeiculoRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(veiculo);

        return repository;
    }

    private static RemoverVeiculoUseCase CriarUseCase(Mock<IVeiculoRepository> repository)
    {
        return new RemoverVeiculoUseCase(repository.Object, new RemoverVeiculoValidator(), MapperFactory.Criar());
    }
}