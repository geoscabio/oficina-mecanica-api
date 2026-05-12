using OficinaMecanica.Domain.Atendimento.Aggregates;

namespace OficinaMecanica.Domain.Atendimento.Interfaces;

public interface IVeiculoRepository
{
    Task AdicionarAsync(Veiculo veiculo, CancellationToken cancellationToken = default);
    Task<Veiculo?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Veiculo?> ObterPorPlacaAsync(string placa, CancellationToken cancellationToken = default);
}
