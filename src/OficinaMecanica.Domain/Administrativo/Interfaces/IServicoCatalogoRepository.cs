using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Domain.Administrativo.Interfaces;

public interface IServicoCatalogoRepository
{
    Task AdicionarAsync(ServicoCatalogo servico, CancellationToken cancellationToken = default);
    Task AtualizarAsync(ServicoCatalogo servico, CancellationToken cancellationToken = default);
    Task RemoverAsync(ServicoCatalogo servico, CancellationToken cancellationToken = default);
    Task<ServicoCatalogo?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ServicoCatalogo>> ListarAsync(
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken = default);

    Task<int> ContarAsync(CancellationToken cancellationToken = default);
}
