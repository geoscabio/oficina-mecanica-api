using FluentAssertions;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.CadastrarVeiculo;
using OficinaMecanica.Application.Common.Exceptions;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Application.UnitTests.Atendimento.VeiculoUseCases.CadastrarVeiculo;

public class CadastrarVeiculoUseCaseTests
{
    [Fact]
    public async Task Dado_RequestValido_Quando_CadastrarVeiculo_Entao_DevePersistirVeiculoERetornarSucesso()
    {
        var cliente = CriarCliente();
        var clienteRepository = new ClienteRepositoryFake(cliente);
        var veiculoRepository = new VeiculoRepositoryFake();
        var useCase = new CadastrarVeiculoUseCase(veiculoRepository, clienteRepository, new CadastrarVeiculoValidator());
        var request = CriarRequestValido(cliente.Id);

        var resultado = await useCase.ExecuteAsync(request);

        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().NotBeEmpty();
        resultado.Valor.ClienteId.Should().Be(cliente.Id);
        resultado.Valor.Placa.Should().Be("ABC1234");
        resultado.Valor.Marca.Should().Be("Toyota");
        resultado.Valor.Modelo.Should().Be("Corolla");
        resultado.Valor.Ano.Should().Be(2020);
        veiculoRepository.Veiculos.Should().ContainSingle();
    }

    [Fact]
    public async Task Dado_ClienteInexistente_Quando_CadastrarVeiculo_Entao_DeveRetornarFalha()
    {
        var clienteRepository = new ClienteRepositoryFake();
        var veiculoRepository = new VeiculoRepositoryFake();
        var useCase = new CadastrarVeiculoUseCase(veiculoRepository, clienteRepository, new CadastrarVeiculoValidator());
        var request = CriarRequestValido(Guid.NewGuid());

        var resultado = await useCase.ExecuteAsync(request);

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Be("Cliente nao encontrado.");
        veiculoRepository.Veiculos.Should().BeEmpty();
    }

    [Fact]
    public async Task Dado_PlacaJaCadastrada_Quando_CadastrarVeiculo_Entao_DeveRetornarFalha()
    {
        var cliente = CriarCliente();
        var clienteRepository = new ClienteRepositoryFake(cliente);
        var veiculoRepository = new VeiculoRepositoryFake();
        var useCase = new CadastrarVeiculoUseCase(veiculoRepository, clienteRepository, new CadastrarVeiculoValidator());
        var request = CriarRequestValido(cliente.Id);
        await useCase.ExecuteAsync(request);

        var resultado = await useCase.ExecuteAsync(request);

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Be("Veiculo ja cadastrado para a placa informada.");
        veiculoRepository.Veiculos.Should().ContainSingle();
    }

    [Fact]
    public async Task Dado_ClienteIdVazio_Quando_CadastrarVeiculo_Entao_DeveLancarValidationException()
    {
        var clienteRepository = new ClienteRepositoryFake();
        var veiculoRepository = new VeiculoRepositoryFake();
        var useCase = new CadastrarVeiculoUseCase(veiculoRepository, clienteRepository, new CadastrarVeiculoValidator());
        var request = CriarRequestValido(Guid.Empty);

        var acao = () => useCase.ExecuteAsync(request);

        await acao.Should().ThrowAsync<ValidationException>();
    }

    private static CadastrarVeiculoRequest CriarRequestValido(Guid clienteId)
    {
        return new CadastrarVeiculoRequest(
            ClienteId: clienteId,
            Placa: "ABC-1234",
            Marca: "Toyota",
            Modelo: "Corolla",
            Ano: 2020);
    }

    private static Cliente CriarCliente()
    {
        return Cliente.Criar(
            CpfCnpj.Criar("529.982.247-25"),
            "Maria Silva",
            new Endereco("Rua A", "100", "Centro", "Sao Paulo", "01001000"),
            Telefone.Criar("(11) 99999-9999"),
            Email.Criar("maria@email.com"));
    }

    private sealed class ClienteRepositoryFake : IClienteRepository
    {
        private readonly List<Cliente> _clientes = new();

        public ClienteRepositoryFake(params Cliente[] clientes)
        {
            _clientes.AddRange(clientes);
        }

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

    private sealed class VeiculoRepositoryFake : IVeiculoRepository
    {
        private readonly List<Veiculo> _veiculos = new();

        public IReadOnlyCollection<Veiculo> Veiculos => _veiculos.AsReadOnly();

        public Task AdicionarAsync(Veiculo veiculo, CancellationToken cancellationToken = default)
        {
            _veiculos.Add(veiculo);
            return Task.CompletedTask;
        }

        public Task<Veiculo?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_veiculos.SingleOrDefault(veiculo => veiculo.Id == id));
        }

        public Task<Veiculo?> ObterPorPlacaAsync(string placa, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_veiculos.SingleOrDefault(veiculo => veiculo.Placa.NumeroPlaca == placa));
        }
    }
}
