using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;

namespace OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;

public interface IOrdemServicoRepository
{
    Task AdicionarAsync(OrdemServico ordemServico, CancellationToken cancellationToken = default);
    Task AtualizarAsync(OrdemServico ordemServico, CancellationToken cancellationToken = default);
    Task<OrdemServico?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<OrdemServico>> ListarAsync(
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken = default);

    Task<int> ContarAsync(CancellationToken cancellationToken = default);
}
