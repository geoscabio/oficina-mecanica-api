using Microsoft.EntityFrameworkCore;
using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Entities;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;
using OficinaMecanica.Infrastructure.Persistence;

namespace OficinaMecanica.Infrastructure.GestaoEstoque.Repositories;

public sealed class EstoqueRepository : IEstoqueRepository
{
    private readonly OficinaMecanicaDbContext _dbContext;

    public EstoqueRepository(OficinaMecanicaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Estoque?> ObterAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Estoques
            .Include(estoque => estoque.ItensEstoque)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Estoque estoque, CancellationToken cancellationToken = default)
    {
        var estoqueExiste = await _dbContext.Estoques
            .AnyAsync(item => item.Id == estoque.Id, cancellationToken);

        if (estoqueExiste)
        {
            _dbContext.Estoques.Update(estoque);
        }
        else
        {
            await _dbContext.Estoques.AddAsync(estoque, cancellationToken);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ItemEstoque>> ListarItensAsync(
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<ItemEstoque>()
            .AsNoTracking()
            .OrderBy(item => item.PecaInsumoCatalogoId)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> ContarItensAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Set<ItemEstoque>().CountAsync(cancellationToken);
    }

    public Task<ItemEstoque?> ObterItemPorIdAsync(
        Guid itemEstoqueId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Set<ItemEstoque>()
            .SingleOrDefaultAsync(item => item.Id == itemEstoqueId, cancellationToken);
    }

    public Task<ItemEstoque?> ObterItemPorPecaInsumoCatalogoIdAsync(
        Guid pecaInsumoCatalogoId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Set<ItemEstoque>()
            .SingleOrDefaultAsync(
                item => item.PecaInsumoCatalogoId == pecaInsumoCatalogoId,
                cancellationToken);
    }

    public async Task AtualizarItemAsync(ItemEstoque itemEstoque, CancellationToken cancellationToken = default)
    {
        _dbContext.Set<ItemEstoque>().Update(itemEstoque);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
