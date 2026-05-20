using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Domain.Administrativo.Interfaces;

public interface IMecanicoRepository
{
    Task AdicionarAsync(Mecanico mecanico, CancellationToken cancellationToken = default);

    Task<Mecanico?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Mecanico>> ListarAsync(int pagina, int tamanhoPagina, CancellationToken cancellationToken = default);

    Task AtualizarAsync(Mecanico mecanico, CancellationToken cancellationToken = default);

    Task RemoverAsync(Mecanico mecanico, CancellationToken cancellationToken = default);

    Task<int> ContarAsync(CancellationToken cancellationToken = default);
}
