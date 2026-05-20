using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;
using OficinaMecanica.Application.Atendimento.ValidationMessages;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.UnitTests.Atendimento.Factories;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.Messages;

namespace OficinaMecanica.Application.UnitTests.Atendimento.ClienteUseCases.CadastrarCliente;

public class CadastrarClienteUseCaseTests
{
    [Fact]
    public async Task Dado_RequestValido_Quando_CadastrarCliente_Entao_DevePersistirClienteERetornarSucesso()
    {
        // Arrange
        var repository = CriarRepository(null);

        var useCase = CriarUseCase(repository);

        var request = ClienteTestDataFactory.CriarCadastrarClienteRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().NotBeEmpty();
        resultado.Valor.Documento.Should().Be(ClienteTestDataFactory.DocumentoNormalizadoPadrao);
        resultado.Valor.Nome.Should().Be(ClienteTestDataFactory.NomePadrao);

        repository.Verify(
            repo => repo.AdicionarAsync(
                It.Is<Cliente>(cliente =>
                    cliente.Documento.Numero == ClienteTestDataFactory.DocumentoNormalizadoPadrao
                    && cliente.Nome == ClienteTestDataFactory.NomePadrao),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Dado_DocumentoJaCadastrado_Quando_CadastrarCliente_Entao_DeveRetornarFalha()
    {
        // Arrange
        var clienteExistente = ClienteTestDataFactory.CriarClientePadrao();

        var repository = CriarRepository(clienteExistente);

        var useCase = CriarUseCase(repository);

        var request = ClienteTestDataFactory.CriarCadastrarClienteRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().Be(ClienteErrorMessages.ClienteDuplicado);
        resultado.Erro.Tipo.Should().Be(TipoErro.RegraNegocio);

        repository.Verify(
            repo => repo.AdicionarAsync(
                It.IsAny<Cliente>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_NomeInvalido_Quando_CadastrarCliente_Entao_DeveRetornarFalhaDeValidacao(string nome)
    {
        // Arrange
        var repository = new Mock<IClienteRepository>();

        var useCase = CriarUseCase(repository);

        var request = ClienteTestDataFactory.CriarCadastrarClienteRequestValido(
            nome: nome);

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

        repository.Verify(
            repo => repo.AdicionarAsync(
                It.IsAny<Cliente>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Dado_MultiplosCamposInvalidos_Quando_CadastrarCliente_Entao_DeveRetornarTodosErrosDeValidacao()
    {
        // Arrange
        var repository = new Mock<IClienteRepository>();

        var useCase = CriarUseCase(repository);

        var request = ClienteTestDataFactory.CriarCadastrarClienteRequestValido(
            documento: string.Empty,
            nome: string.Empty,
            telefone: string.Empty,
            email: string.Empty);

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Tipo.Should().Be(TipoErro.Validacao);
        resultado.Erro.Erros.Should().BeEquivalentTo(
            ClienteValidationMessages.DocumentoObrigatorio,
            ClienteValidationMessages.NomeObrigatorio,
            ClienteValidationMessages.TelefoneObrigatorio,
            ClienteValidationMessages.EmailObrigatorio);

        repository.Verify(
            repo => repo.AdicionarAsync(
                It.IsAny<Cliente>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static Mock<IClienteRepository> CriarRepository(Cliente? cliente)
    {
        var repository = new Mock<IClienteRepository>();

        repository
            .Setup(repo => repo.ObterPorDocumentoAsync(
                ClienteTestDataFactory.DocumentoNormalizadoPadrao,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        return repository;
    }

    private static CadastrarClienteUseCase CriarUseCase(Mock<IClienteRepository> repository)
    {
        return new CadastrarClienteUseCase(
            repository.Object,
            new CadastrarClienteValidator(),
            MapperFactory.Criar());
    }
}
