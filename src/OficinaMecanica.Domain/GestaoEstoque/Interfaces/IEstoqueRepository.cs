using OficinaMecanica.Domain.GestaoEstoque.Aggregates;

namespace OficinaMecanica.Domain.GestaoEstoque.Interfaces;

public interface IEstoqueRepository
{
    Task<Estoque?> ObterAsync(CancellationToken cancellationToken = default);
    Task AtualizarAsync(Estoque estoque, CancellationToken cancellationToken = default);
}
