using FluentAssertions;
using FluentValidation;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;
using OficinaMecanica.Application.UnitTests.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.ValueObjects;
using Moq;

namespace OficinaMecanica.Application.UnitTests.Atendimento.ClienteUseCases.CadastrarCliente;

public class CadastrarClienteUseCaseTests
{
    [Fact]
    public async Task Dado_RequestValido_Quando_CadastrarCliente_Entao_DevePersistirClienteERetornarSucesso()
    {
        var repository = new Mock<IClienteRepository>();
        repository
            .Setup(repo => repo.ObterPorDocumentoAsync("52998224725", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Cliente?)null);
        var useCase = new CadastrarClienteUseCase(repository.Object, new CadastrarClienteValidator(), MapperFactory.Criar());
        var request = CriarRequestValido();

        var resultado = await useCase.ExecuteAsync(request);

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
        var clienteExistente = CriarCliente();
        var repository = new Mock<IClienteRepository>();
        repository
            .Setup(repo => repo.ObterPorDocumentoAsync("52998224725", It.IsAny<CancellationToken>()))
            .ReturnsAsync(clienteExistente);
        var useCase = new CadastrarClienteUseCase(repository.Object, new CadastrarClienteValidator(), MapperFactory.Criar());
        var request = CriarRequestValido();

        var resultado = await useCase.ExecuteAsync(request);

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Be("Cliente ja cadastrado para o documento informado.");
        repository.Verify(repo => repo.AdicionarAsync(It.IsAny<Cliente>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_NomeInvalido_Quando_CadastrarCliente_Entao_DeveLancarValidationException(string nome)
    {
        var repository = new Mock<IClienteRepository>();
        var useCase = new CadastrarClienteUseCase(repository.Object, new CadastrarClienteValidator(), MapperFactory.Criar());
        var request = CriarRequestValido() with { Nome = nome };

        var acao = () => useCase.ExecuteAsync(request);

        await acao.Should().ThrowAsync<ValidationException>();
    }

    private static CadastrarClienteRequest CriarRequestValido()
    {
        return new CadastrarClienteRequest(
            Documento: "529.982.247-25",
            Nome: "Maria Silva",
            Logradouro: "Rua A",
            Numero: "100",
            Bairro: "Centro",
            Cidade: "Sao Paulo",
            CEP: "01001-000",
            Telefone: "(11) 99999-9999",
            Email: "maria@email.com");
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
