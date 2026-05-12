using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Domain.Administrativo.Interfaces;

public interface IMecanicoRepository
{
    Task AdicionarAsync(Mecanico mecanico, CancellationToken cancellationToken = default);
    Task<Mecanico?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
}
