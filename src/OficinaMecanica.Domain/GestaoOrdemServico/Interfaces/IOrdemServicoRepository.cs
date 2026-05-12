using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;

namespace OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;

public interface IOrdemServicoRepository
{
    Task AdicionarAsync(OrdemServico ordemServico, CancellationToken cancellationToken = default);
    Task AtualizarAsync(OrdemServico ordemServico, CancellationToken cancellationToken = default);
    Task<OrdemServico?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
}
