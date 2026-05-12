using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Domain.Administrativo.Interfaces;

public interface IPecaInsumoCatalogoRepository
{
    Task AdicionarAsync(PecaInsumoCatalogo pecaInsumo, CancellationToken cancellationToken = default);
    Task<PecaInsumoCatalogo?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
}
