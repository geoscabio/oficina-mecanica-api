using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarClientePorDocumento;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Atendimento.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.Messages;

namespace OficinaMecanica.Application.UnitTests.Atendimento.ClienteUseCases.ConsultarClientePorDocumento;

public class ConsultarClientePorDocumentoUseCaseTests
{
    [Fact]
    public async Task Dado_ClienteExistente_Quando_ConsultarClientePorDocumento_Entao_DeveRetornarDadosDoCliente()
    {
        // Arrange
        var cliente = ClienteTestDataFactory.CriarClientePadrao();

        var repository = CriarRepository(cliente);

        var useCase = CriarUseCase(repository);

        var request = ClienteTestDataFactory.CriarConsultarClientePorDocumentoRequestValido();

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
            repo => repo.ObterPorDocumentoAsync(
                ClienteTestDataFactory.DocumentoNormalizadoPadrao,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_ClienteInexistente_Quando_ConsultarClientePorDocumento_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request = ClienteTestDataFactory.CriarConsultarClientePorDocumentoRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();

        resultado.Erro.Should().NotBeNull();

        resultado.Erro!.Mensagem.Should().Be(ClienteErrorMessages.ClienteNaoEncontrado);

        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);

        repository.Verify(
            repo => repo.ObterPorDocumentoAsync(
                ClienteTestDataFactory.DocumentoNormalizadoPadrao,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_DocumentoVazio_Quando_ConsultarClientePorDocumento_Entao_DeveRetornarFalhaDeValidacao(
        string documento)
    {
        // Arrange
        var repository = new Mock<IClienteRepository>();

        var useCase = CriarUseCase(repository);

        var request = ClienteTestDataFactory.CriarConsultarClientePorDocumentoRequestValido(
            documento);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();

        resultado.Erro.Should().NotBeNull();

        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();

        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);

        repository.Verify(
            repo => repo.ObterPorDocumentoAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Mock<IClienteRepository> CriarRepository(Cliente? cliente)
    {
        var repository = new Mock<IClienteRepository>();

        repository
            .Setup(repo => repo.ObterPorDocumentoAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        return repository;
    }

    private static ConsultarClientePorDocumentoUseCase CriarUseCase(
        Mock<IClienteRepository> repository)
    {
        return new ConsultarClientePorDocumentoUseCase(
            repository.Object,
            new ConsultarClientePorDocumentoValidator(),
            MapperFactory.Criar());
    }
}