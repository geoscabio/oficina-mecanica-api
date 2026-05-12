using FluentAssertions;
using FluentValidation;
using Moq;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarClientePorDocumento;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Application.UnitTests.Atendimento.ClienteUseCases.ConsultarClientePorDocumento;

public class ConsultarClientePorDocumentoUseCaseTests
{
    [Fact]
    public async Task Dado_ClienteExistente_Quando_ConsultarClientePorDocumento_Entao_DeveRetornarDadosDoCliente()
    {
        var cliente = CriarCliente();
        var repository = new Mock<IClienteRepository>();
        repository
            .Setup(repo => repo.ObterPorDocumentoAsync("52998224725", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cliente);
        var useCase = new ConsultarClientePorDocumentoUseCase(
            repository.Object,
            new ConsultarClientePorDocumentoValidator(),
            MapperFactory.Criar());

        var resultado = await useCase.ExecuteAsync(new ConsultarClientePorDocumentoRequest("529.982.247-25"));

        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(cliente.Id);
        resultado.Valor.Documento.Should().Be("52998224725");
        resultado.Valor.Nome.Should().Be("Maria Silva");
        resultado.Valor.Logradouro.Should().Be("Rua A");
        resultado.Valor.Numero.Should().Be("100");
        resultado.Valor.Bairro.Should().Be("Centro");
        resultado.Valor.Cidade.Should().Be("Sao Paulo");
        resultado.Valor.CEP.Should().Be("01001000");
        resultado.Valor.Telefone.Should().Be("11999999999");
        resultado.Valor.Email.Should().Be("maria@email.com");
    }

    [Fact]
    public async Task Dado_ClienteInexistente_Quando_ConsultarClientePorDocumento_Entao_DeveRetornarFalha()
    {
        var repository = new Mock<IClienteRepository>();
        repository
            .Setup(repo => repo.ObterPorDocumentoAsync("52998224725", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente?)null);
        var useCase = new ConsultarClientePorDocumentoUseCase(
            repository.Object,
            new ConsultarClientePorDocumentoValidator(),
            MapperFactory.Criar());

        var resultado = await useCase.ExecuteAsync(new ConsultarClientePorDocumentoRequest("529.982.247-25"));

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Be("Cliente nao encontrado.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_DocumentoVazio_Quando_ConsultarClientePorDocumento_Entao_DeveLancarValidationException(
        string documento)
    {
        var repository = new Mock<IClienteRepository>();
        var useCase = new ConsultarClientePorDocumentoUseCase(
            repository.Object,
            new ConsultarClientePorDocumentoValidator(),
            MapperFactory.Criar());

        var acao = () => useCase.ExecuteAsync(new ConsultarClientePorDocumentoRequest(documento));

        await acao.Should().ThrowAsync<ValidationException>();
    }

    private static Cliente CriarCliente()
    {
        return Cliente.Criar(
            CpfCnpj.Criar("529.982.247-25"),
            "Maria Silva",
            new Endereco("Rua A", "100", "Centro", "Sao Paulo", "01001-000"),
            Telefone.Criar("(11) 99999-9999"),
            Email.Criar("maria@email.com"));
    }
}
