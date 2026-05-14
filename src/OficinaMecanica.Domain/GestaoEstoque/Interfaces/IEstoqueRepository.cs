using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Entities;

namespace OficinaMecanica.Domain.GestaoEstoque.Interfaces;

public interface IEstoqueRepository
{
    Task<Estoque?> ObterAsync(CancellationToken cancellationToken = default);

    Task AtualizarAsync(Estoque estoque, CancellationToken cancellationToken = default);

    Task<ItemEstoque?> ObterItemPorPecaInsumoCatalogoIdAsync(Guid pecaInsumoCatalogoId, CancellationToken cancellationToken = default);

    Task AtualizarItemAsync(ItemEstoque itemEstoque, CancellationToken cancellationToken = default);
}