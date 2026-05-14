using OficinaMecanica.Domain.Atendimento.Aggregates;

namespace OficinaMecanica.Domain.Atendimento.Interfaces;

public interface IVeiculoRepository
{
    Task AdicionarAsync(Veiculo veiculo, CancellationToken cancellationToken = default);

    Task<Veiculo?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Veiculo?> ObterPorPlacaAsync(string placa, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Veiculo>> ListarAsync(int pagina, int tamanhoPagina, CancellationToken cancellationToken = default);

    Task AtualizarAsync(Veiculo veiculo, CancellationToken cancellationToken = default);

    Task RemoverAsync(Veiculo veiculo, CancellationToken cancellationToken = default);

    Task<int> ContarAsync(CancellationToken cancellationToken = default);
}
