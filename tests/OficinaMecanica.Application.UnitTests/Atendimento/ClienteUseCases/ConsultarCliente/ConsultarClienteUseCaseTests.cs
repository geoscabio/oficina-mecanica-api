using FluentAssertions;
using Moq;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarCliente;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Application.UnitTests.Atendimento.Builders;
using OficinaMecanica.Domain.Atendimento.Interfaces;

namespace OficinaMecanica.Application.UnitTests.Atendimento.ClienteUseCases.ConsultarCliente;

public class ConsultarClienteUseCaseTests
{
    [Fact]
    public async Task Dado_ClienteExistente_Quando_ConsultarCliente_Entao_DeveRetornarDadosDoCliente()
    {
        // Arrange
        var cliente = ClienteTestDataFactory.CriarClientePadrao();
        var repository = new Mock<IClienteRepository>();
        repository
            .Setup(repo => repo.ObterPorIdAsync(cliente.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);

        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarClienteRequest(cliente.Id));

        // Assert
        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(cliente.Id);
        resultado.Valor.Documento.Should().Be("52998224725");
        resultado.Valor.Nome.Should().Be("Maria Silva");
        resultado.Valor.Endereco.Logradouro.Should().Be("Rua A");
        resultado.Valor.Endereco.Numero.Should().Be("100");
        resultado.Valor.Endereco.Bairro.Should().Be("Centro");
        resultado.Valor.Endereco.Cidade.Should().Be("Sao Paulo");
        resultado.Valor.Endereco.CEP.Should().Be("01001000");
        resultado.Valor.Telefone.Should().Be("11999999999");
        resultado.Valor.Email.Should().Be("maria@email.com");
    }

    [Fact]
    public async Task Dado_ClienteInexistente_Quando_ConsultarCliente_Entao_DeveRetornarFalha()
    {
        // Arrange
        var repository = new Mock<IClienteRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarClienteRequest(Guid.NewGuid()));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro!.Mensagem.Should().Be(ClienteErrorMessages.ClienteNaoEncontrado);
        resultado.Erro.Tipo.Should().Be(TipoErro.NaoEncontrado);
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_ConsultarCliente_Entao_DeveRetornarFalhaDeValidacao()
    {
        // Arrange
        var repository = new Mock<IClienteRepository>();
        var useCase = CriarUseCase(repository);

        // Act
        var resultado = await useCase.ExecuteAsync(new ConsultarClienteRequest(Guid.Empty));

        // Assert
        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().NotBeNull();
        resultado.Erro!.Mensagem.Should().NotBeNullOrWhiteSpace();
        resultado.Erro.Tipo.Should().Be(TipoErro.Validacao);
    }

    private static ConsultarClienteUseCase CriarUseCase(Mock<IClienteRepository> repository)
    {
        return new ConsultarClienteUseCase(
            repository.Object,
            new ConsultarClienteValidator(),
            MapperFactory.Criar());
    }
}







