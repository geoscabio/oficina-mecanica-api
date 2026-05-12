using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Domain.Administrativo.Interfaces;

public interface IServicoCatalogoRepository
{
    Task AdicionarAsync(ServicoCatalogo servico, CancellationToken cancellationToken = default);
    Task<ServicoCatalogo?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
}
