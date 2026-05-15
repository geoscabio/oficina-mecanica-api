using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.Atendimento.Factories;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;

namespace OficinaMecanica.Application.UnitTests.Atendimento.ClienteUseCases.CadastrarCliente;

public class CadastrarClienteUseCaseTests
{
    [Fact]
    public async Task Dado_RequestValido_Quando_CadastrarCliente_Entao_DevePersistirClienteERetornarSucesso()
    {
        // Arrange
        var repository = new Mock<IClienteRepository>();
        repository
            .Setup(repo => repo.ObterPorDocumentoAsync("52998224725", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente?)null);

        var useCase = CriarUseCase(repository);
        var request = CriarRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().NotBeEmpty();
        resultado.Valor.Documento.Should().Be("52998224725");
        resultado.Valor.Nome.Should().Be("Maria Silva");

        repository.Verify(repo => repo.AdicionarAsync(It.Is<Cliente>(cliente =>
            cliente.Documento.Numero == "52998224725"
            && cliente.Nome == "Maria Silva"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Dado_DocumentoJaCadastrado_Quando_CadastrarCliente_Entao_DeveRetornarFalha()
    {
        // Arrange
        var clienteExistente = ClienteTestDataFactory.CriarClientePadrao();
        var repository = new Mock<IClienteRepository>();
        repository
            .Setup(repo => repo.ObterPorDocumentoAsync("52998224725", It.IsAny<CancellationToken>()))
            .ReturnsAsync(clienteExistente);

        var useCase = CriarUseCase(repository);
        var request = CriarRequestValido();

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(ClienteErrorMessages.ClienteDuplicado);
        resultado.Erro.Tipo.Should().Be(TipoErro.RegraNegocio);

        repository.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_NomeInvalido_Quando_CadastrarCliente_Entao_DeveRetornarFalhaDeValidacao(string nome)
    {
        // Arrange
        var repository = new Mock<IClienteRepository>();
        var useCase = CriarUseCase(repository);
        var request = CriarRequestValido() with { Nome = nome };

        // Act
        var resultado = await useCase.ExecuteAsync(request);

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static CadastrarClienteUseCase CriarUseCase(Mock<IClienteRepository> repository)
    {
        return new CadastrarClienteUseCase(
            repository.Object,
            new CadastrarClienteValidator(),
            MapperFactory.Criar());
    }

    private static CadastrarClienteRequest CriarRequestValido()
    {
        return new CadastrarClienteRequest(
            Documento: "529.982.247-25",
            Nome: "Maria Silva",
            Endereco: new EnderecoRequest(
                Logradouro: "Rua A",
                Numero: "100",
                Bairro: "Centro",
                Cidade: "Sao Paulo",
                CEP: "01001-000"),
            Telefone: "(11) 99999-9999",
            Email: "maria@email.com");
    }
}







