using FluentAssertions;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;
using OficinaMecanica.Application.Common.Exceptions;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;

namespace OficinaMecanica.Application.UnitTests.Atendimento.ClienteUseCases.CadastrarCliente;

public class CadastrarClienteUseCaseTests
{
    [Fact]
    public async Task Dado_RequestValido_Quando_CadastrarCliente_Entao_DevePersistirClienteERetornarSucesso()
    {
        var repository = new ClienteRepositoryFake();
        var useCase = new CadastrarClienteUseCase(repository, new CadastrarClienteValidator());
        var request = CriarRequestValido();

        var resultado = await useCase.ExecuteAsync(request);

        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().NotBeEmpty();
        resultado.Valor.Documento.Should().Be("52998224725");
        resultado.Valor.Nome.Should().Be("Maria Silva");
        repository.Clientes.Should().ContainSingle();
    }

    [Fact]
    public async Task Dado_DocumentoJaCadastrado_Quando_CadastrarCliente_Entao_DeveRetornarFalha()
    {
        var repository = new ClienteRepositoryFake();
        var useCase = new CadastrarClienteUseCase(repository, new CadastrarClienteValidator());
        var request = CriarRequestValido();
        await useCase.ExecuteAsync(request);

        var resultado = await useCase.ExecuteAsync(request);

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Be("Cliente ja cadastrado para o documento informado.");
        repository.Clientes.Should().ContainSingle();
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    public async Task Dado_NomeInvalido_Quando_CadastrarCliente_Entao_DeveLancarValidationException(string nome)
    {
        var repository = new ClienteRepositoryFake();
        var useCase = new CadastrarClienteUseCase(repository, new CadastrarClienteValidator());
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

    private sealed class ClienteRepositoryFake : IClienteRepository
    {
        private readonly List<Cliente> _clientes = new();

        public IReadOnlyCollection<Cliente> Clientes => _clientes.AsReadOnly();

        public Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default)
        {
            _clientes.Add(cliente);
            return Task.CompletedTask;
        }

        public Task<Cliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_clientes.SingleOrDefault(cliente => cliente.Id == id));
        }

        public Task<Cliente?> ObterPorDocumentoAsync(string documento, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_clientes.SingleOrDefault(cliente => cliente.Documento.Numero == documento));
        }
    }
}
