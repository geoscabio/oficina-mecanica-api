using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.AtualizarCliente;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;
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
        var repository = new Mock<IClienteRepository>();

        repository
            .Setup(repo => repo.ObterPorIdAsync(cliente.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        var useCase = CriarUseCase(repository);
        var request = CriarRequestValido(cliente.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(cliente.Id);
        resultado.Valor.Nome.Should().Be(request.Nome);
        resultado.Valor.Telefone.Should().Be("11988887777");
        resultado.Valor.Email.Should().Be(request.Email);

        repository.Verify(
            repo => repo.AtualizarAsync(
                It.Is<Cliente>(clienteAtualizado =>
                    clienteAtualizado.Id == cliente.Id
                    && clienteAtualizado.Nome == request.Nome
                    && clienteAtualizado.Telefone.Numero == "11988887777"
                    && clienteAtualizado.Email.Endereco == request.Email),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_ClienteInexistente_Quando_AtualizarCliente_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = new Mock<IClienteRepository>();
        var useCase = CriarUseCase(repository);
        var request = CriarRequestValido(Guid.NewGuid());

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(ClienteErrorMessages.ClienteNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(
            repo => repo.AtualizarAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_AtualizarCliente_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IClienteRepository>();
        var useCase = CriarUseCase(repository);
        var request = CriarRequestValido(Guid.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_NomeInvalido_Quando_AtualizarCliente_Entao_DeveRetornarFalhaDeValidacao(string nome)
    {
        // Arrange
        var repository = new Mock<IClienteRepository>();
        var useCase = CriarUseCase(repository);
        var request = CriarRequestValido(Guid.NewGuid()) with { Nome = nome };

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static AtualizarClienteUseCase CriarUseCase(Mock<IClienteRepository> repository)
    {
        return new AtualizarClienteUseCase(
            repository.Object,
            new AtualizarClienteValidator(),
            MapperFactory.Criar());
    }

    private static AtualizarClienteRequest CriarRequestValido(Guid clienteId)
    {
        return new AtualizarClienteRequest(
            ClienteId: clienteId,
            Nome: "Cliente Atualizado",
            Endereco: new EnderecoRequest(
                Logradouro: "Rua B",
                Numero: "200",
                Bairro: "Bairro Novo",
                Cidade: "Santo Andre",
                CEP: "09000-000"),
            Telefone: "(11) 98888-7777",
            Email: "novo@email.com");
    }
}