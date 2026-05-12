using FluentAssertions;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarCliente;
using OficinaMecanica.Application.Common.Exceptions;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Application.UnitTests.Atendimento.ClienteUseCases.ConsultarCliente;

public class ConsultarClienteUseCaseTests
{
    [Fact]
    public async Task Dado_ClienteExistente_Quando_ConsultarCliente_Entao_DeveRetornarDadosDoCliente()
    {
        var repository = new ClienteRepositoryFake();
        var cliente = CriarCliente();
        await repository.AdicionarAsync(cliente);
        var useCase = new ConsultarClienteUseCase(repository, new ConsultarClienteValidator());

        var resultado = await useCase.ExecuteAsync(new ConsultarClienteRequest(cliente.Id));

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
    public async Task Dado_ClienteInexistente_Quando_ConsultarCliente_Entao_DeveRetornarFalha()
    {
        var repository = new ClienteRepositoryFake();
        var useCase = new ConsultarClienteUseCase(repository, new ConsultarClienteValidator());

        var resultado = await useCase.ExecuteAsync(new ConsultarClienteRequest(Guid.NewGuid()));

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Be("Cliente nao encontrado.");
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_ConsultarCliente_Entao_DeveLancarValidationException()
    {
        var repository = new ClienteRepositoryFake();
        var useCase = new ConsultarClienteUseCase(repository, new ConsultarClienteValidator());

        var acao = () => useCase.ExecuteAsync(new ConsultarClienteRequest(Guid.Empty));

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

    private sealed class ClienteRepositoryFake : IClienteRepository
    {
        private readonly List<Cliente> _clientes = new();

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
