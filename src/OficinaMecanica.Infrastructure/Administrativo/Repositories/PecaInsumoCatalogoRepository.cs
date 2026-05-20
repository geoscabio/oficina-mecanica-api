using Microsoft.EntityFrameworkCore;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Infrastructure.Persistence;

namespace OficinaMecanica.Infrastructure.Administrativo.Repositories;

public sealed class PecaInsumoCatalogoRepository : IPecaInsumoCatalogoRepository
{
    private readonly OficinaMecanicaDbContext _dbContext;

    public PecaInsumoCatalogoRepository(OficinaMecanicaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AdicionarAsync(PecaInsumoCatalogo pecaInsumo, CancellationToken cancellationToken = default)
    {
        await _dbContext.PecasInsumosCatalogo.AddAsync(pecaInsumo, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<PecaInsumoCatalogo?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.PecasInsumosCatalogo
            .SingleOrDefaultAsync(pecaInsumo => pecaInsumo.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<PecaInsumoCatalogo>> ObterPorIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.PecasInsumosCatalogo
            .Where(pecaInsumo => ids.Contains(pecaInsumo.Id))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<PecaInsumoCatalogo>> ListarAsync(
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.PecasInsumosCatalogo
            .AsNoTracking()
            .OrderBy(pecaInsumo => pecaInsumo.Descricao)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AtualizarAsync(
        PecaInsumoCatalogo pecaInsumoCatalogo,
        CancellationToken cancellationToken = default)
    {
        _dbContext.PecasInsumosCatalogo.Update(pecaInsumoCatalogo);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(
        PecaInsumoCatalogo pecaInsumoCatalogo,
        CancellationToken cancellationToken = default)
    {
        _dbContext.PecasInsumosCatalogo.Remove(pecaInsumoCatalogo);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<int> ContarAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.PecasInsumosCatalogo.CountAsync(cancellationToken);
    }
}
