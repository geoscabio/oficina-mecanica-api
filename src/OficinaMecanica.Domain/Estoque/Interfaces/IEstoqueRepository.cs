using EstoqueAggregate = OficinaMecanica.Domain.Estoque.Aggregates.Estoque;

namespace OficinaMecanica.Domain.Estoque.Interfaces;

public interface IEstoqueRepository
{
    Task<EstoqueAggregate?> ObterAsync(CancellationToken cancellationToken = default);
    Task AtualizarAsync(EstoqueAggregate estoque, CancellationToken cancellationToken = default);
}
