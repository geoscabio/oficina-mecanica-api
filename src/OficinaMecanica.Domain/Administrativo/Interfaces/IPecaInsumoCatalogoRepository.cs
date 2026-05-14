using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Domain.Administrativo.Interfaces;

public interface IPecaInsumoCatalogoRepository
{
    Task AdicionarAsync(PecaInsumoCatalogo pecaInsumo, CancellationToken cancellationToken = default);

    Task<PecaInsumoCatalogo?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<PecaInsumoCatalogo>> ListarAsync(int pagina, int tamanhoPagina, CancellationToken cancellationToken = default);

    Task AtualizarAsync(PecaInsumoCatalogo pecaInsumoCatalogo, CancellationToken cancellationToken = default);

    Task RemoverAsync(PecaInsumoCatalogo pecaInsumoCatalogo, CancellationToken cancellationToken = default);

    Task<int> ContarAsync(CancellationToken cancellationToken = default);
}
