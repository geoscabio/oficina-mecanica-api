using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.RemoverCliente;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Atendimento.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.Messages;

namespace OficinaMecanica.Application.UnitTests.Atendimento.ClienteUseCases.RemoverCliente;

public class RemoverClienteUseCaseTests
{
    [Fact]
    public async Task Dado_ClienteExistente_Quando_RemoverCliente_Entao_DeveRemoverCliente()
    {
        // Arrange
        var cliente = ClienteTestDataFactory.CriarClientePadrao();

        var repository = CriarRepository(cliente);

        var useCase = CriarUseCase(repository);

        var request = ClienteTestDataFactory.CriarRemoverClienteRequestValido(cliente.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(cliente.Id);

        repository.Verify(
            repo => repo.ObterPorIdAsync(
                request.ClienteId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            repo => repo.RemoverAsync(
                It.Is<Cliente>(clienteRemovido => clienteRemovido.Id == cliente.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_ClienteInexistente_Quando_RemoverCliente_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request = ClienteTestDataFactory.CriarRemoverClienteRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(ClienteErrorMessages.ClienteNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(
            repo => repo.ObterPorIdAsync(
                request.ClienteId,
                It.IsAny<CancellationToken>()),
            Times.Once);

        repository.Verify(
            repo => repo.RemoverAsync(
                It.IsAny<Cliente>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_RemoverCliente_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IClienteRepository>();

        var useCase = CriarUseCase(repository);

        var request = ClienteTestDataFactory.CriarRemoverClienteRequestValido(Guid.Empty);

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
            repo => repo.RemoverAsync(
                It.IsAny<Cliente>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Mock<IClienteRepository> CriarRepository(Cliente? cliente)
    {
        var repository = new Mock<IClienteRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        return repository;
    }

    private static RemoverClienteUseCase CriarUseCase(Mock<IClienteRepository> repository)
    {
        return new RemoverClienteUseCase(
            repository.Object,
            new RemoverClienteValidator(),
            MapperFactory.Criar());
    }
}