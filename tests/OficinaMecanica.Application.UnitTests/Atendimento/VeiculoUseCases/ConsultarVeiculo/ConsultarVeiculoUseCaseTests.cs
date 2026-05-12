using FluentAssertions;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculo;
using OficinaMecanica.Application.Common.Exceptions;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Application.UnitTests.Atendimento.VeiculoUseCases.ConsultarVeiculo;

public class ConsultarVeiculoUseCaseTests
{
    [Fact]
    public async Task Dado_VeiculoExistente_Quando_ConsultarVeiculo_Entao_DeveRetornarDadosDoVeiculo()
    {
        var repository = new VeiculoRepositoryFake();
        var veiculo = CriarVeiculo();
        await repository.AdicionarAsync(veiculo);
        var useCase = new ConsultarVeiculoUseCase(repository, new ConsultarVeiculoValidator());

        var resultado = await useCase.ExecuteAsync(new ConsultarVeiculoRequest(veiculo.Id));

        resultado.Sucesso.Should().BeTrue();
        resultado.Valor.Should().NotBeNull();
        resultado.Valor!.Id.Should().Be(veiculo.Id);
        resultado.Valor.ClienteId.Should().Be(veiculo.ClienteId);
        resultado.Valor.Placa.Should().Be("ABC1234");
        resultado.Valor.Marca.Should().Be("Toyota");
        resultado.Valor.Modelo.Should().Be("Corolla");
        resultado.Valor.Ano.Should().Be(2020);
    }

    [Fact]
    public async Task Dado_VeiculoInexistente_Quando_ConsultarVeiculo_Entao_DeveRetornarFalha()
    {
        var repository = new VeiculoRepositoryFake();
        var useCase = new ConsultarVeiculoUseCase(repository, new ConsultarVeiculoValidator());

        var resultado = await useCase.ExecuteAsync(new ConsultarVeiculoRequest(Guid.NewGuid()));

        resultado.Sucesso.Should().BeFalse();
        resultado.Erro.Should().Be("Veiculo nao encontrado.");
    }

    [Fact]
    public async Task Dado_IdVazio_Quando_ConsultarVeiculo_Entao_DeveLancarValidationException()
    {
        var repository = new VeiculoRepositoryFake();
        var useCase = new ConsultarVeiculoUseCase(repository, new ConsultarVeiculoValidator());

        var acao = () => useCase.ExecuteAsync(new ConsultarVeiculoRequest(Guid.Empty));

        await acao.Should().ThrowAsync<ValidationException>();
    }

    private static Veiculo CriarVeiculo()
    {
        return Veiculo.Criar(
            Guid.NewGuid(),
            Placa.Criar("ABC-1234"),
            "Toyota",
            "Corolla",
            2020);
    }

    private sealed class VeiculoRepositoryFake : IVeiculoRepository
    {
        private readonly List<Veiculo> _veiculos = new();

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
