using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarCliente;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Atendimento.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.Messages;

namespace OficinaMecanica.Application.UnitTests.Atendimento.ClienteUseCases.ConsultarCliente;

public class ConsultarClienteUseCaseTests
{
    [Fact]
    public async Task Dado_ClienteExistente_Quando_ConsultarCliente_Entao_DeveRetornarDadosDoCliente()
    {
        // Arrange
        var cliente = ClienteTestDataFactory.CriarClientePadrao();

        var repository = CriarRepository(cliente);

        var useCase = CriarUseCase(repository);

        var request = ClienteTestDataFactory.CriarConsultarClienteRequestValido(cliente.Id);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();

        resultado.Valor.Should().NotBeNull();

        resultado.Valor!.Id.Should().Be(cliente.Id);

        resultado.Valor.Documento.Should().Be(ClienteTestDataFactory.DocumentoNormalizadoPadrao);

        resultado.Valor.Nome.Should().Be(ClienteTestDataFactory.NomePadrao);

        resultado.Valor.Endereco.Logradouro.Should().Be(ClienteTestDataFactory.LogradouroPadrao);

        resultado.Valor.Endereco.Numero.Should().Be(ClienteTestDataFactory.NumeroPadrao);

        resultado.Valor.Endereco.Bairro.Should().Be(ClienteTestDataFactory.BairroPadrao);

        resultado.Valor.Endereco.Cidade.Should().Be(ClienteTestDataFactory.CidadePadrao);

        resultado.Valor.Endereco.CEP.Should().Be(ClienteTestDataFactory.CepNormalizadoPadrao);

        resultado.Valor.Telefone.Should().Be(ClienteTestDataFactory.TelefoneNormalizadoPadrao);

        resultado.Valor.Email.Should().Be(ClienteTestDataFactory.EmailPadrao);

        repository.Verify(
            repo => repo.ObterPorIdAsync(
                request.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_ClienteInexistente_Quando_ConsultarCliente_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request = ClienteTestDataFactory.CriarConsultarClienteRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();

        resultado.Erro.Should().NotBeNull();

        resultado.Erro!.Mensagem.Should().Be(ClienteErrorMessages.ClienteNaoEncontrado);

        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(
            repo => repo.ObterPorIdAsync(
                request.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_ConsultarCliente_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IClienteRepository>();

        var useCase = CriarUseCase(repository);

        var request = ClienteTestDataFactory.CriarConsultarClienteRequestValido(Guid.Empty);

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

    private static ConsultarClienteUseCase CriarUseCase(Mock<IClienteRepository> repository)
    {
        return new ConsultarClienteUseCase(
            repository.Object,
            new ConsultarClienteValidator(),
            MapperFactory.Criar());
    }
}