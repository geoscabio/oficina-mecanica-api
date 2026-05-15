using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.AtualizarCliente;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Atendimento.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.Messages;

namespace OficinaMecanica.Application.UnitTests.Atendimento.ClienteUseCases.AtualizarCliente;

public class AtualizarClienteUseCaseTests
{
    [Fact]
    public async Task Dado_DadosValidos_Quando_AtualizarCliente_Entao_DeveAtualizarCliente()
    {
        // Arrange
        var cliente = ClienteTestDataFactory.CriarClientePadrao();

        var repository = CriarRepository(cliente);

        var useCase = CriarUseCase(repository);

        var request = ClienteTestDataFactory.CriarAtualizarClienteRequestValido(cliente.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(cliente.Id);
        resultado.Valor.Nome.Should().Be(request.Nome);
        resultado.Valor.Telefone.Should().Be(ClienteTestDataFactory.TelefoneAtualizadoNormalizado);
        resultado.Valor.Email.Should().Be(request.Email);

        repository.Verify(
            repo => repo.AtualizarAsync(
                It.Is<Cliente>(clienteAtualizado =>
                    clienteAtualizado.Id == cliente.Id
                    && clienteAtualizado.Nome == request.Nome
                    && clienteAtualizado.Telefone.Numero == ClienteTestDataFactory.TelefoneAtualizadoNormalizado
                    && clienteAtualizado.Email.Endereco == request.Email),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_ClienteInexistente_Quando_AtualizarCliente_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request = ClienteTestDataFactory.CriarAtualizarClienteRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(ClienteErrorMessages.ClienteNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(
            repo => repo.AtualizarAsync(
                It.IsAny<Cliente>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_AtualizarCliente_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IClienteRepository>();

        var useCase = CriarUseCase(repository);

        var request = ClienteTestDataFactory.CriarAtualizarClienteRequestValido(Guid.Empty);

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
                It.IsAny<Cliente>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_NomeInvalido_Quando_AtualizarCliente_Entao_DeveRetornarFalhaDeValidacao(string nome)
    {
        // Arrange
        var repository = new Mock<IClienteRepository>();

        var useCase = CriarUseCase(repository);

        var request = ClienteTestDataFactory.CriarAtualizarClienteRequestValido(
            nome: nome);

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

    private static AtualizarClienteUseCase CriarUseCase(Mock<IClienteRepository> repository)
    {
        return new AtualizarClienteUseCase(
            repository.Object,
            new AtualizarClienteValidator(),
            MapperFactory.Criar());
    }
}