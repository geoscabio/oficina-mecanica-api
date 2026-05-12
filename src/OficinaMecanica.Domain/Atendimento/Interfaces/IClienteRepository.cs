using OficinaMecanica.Domain.Atendimento.Aggregates;

namespace OficinaMecanica.Domain.Atendimento.Interfaces;

public interface IClienteRepository
{
    Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default);
    Task<Cliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Cliente?> ObterPorDocumentoAsync(string documento, CancellationToken cancellationToken = default);
}
