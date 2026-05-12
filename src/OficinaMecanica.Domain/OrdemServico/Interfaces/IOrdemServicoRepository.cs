using OrdemServicoAggregate = OficinaMecanica.Domain.OrdemServico.Aggregates.OrdemServico;

namespace OficinaMecanica.Domain.OrdemServico.Interfaces;

public interface IOrdemServicoRepository
{
    Task AdicionarAsync(OrdemServicoAggregate ordemServico, CancellationToken cancellationToken = default);
    Task<OrdemServicoAggregate?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
}
