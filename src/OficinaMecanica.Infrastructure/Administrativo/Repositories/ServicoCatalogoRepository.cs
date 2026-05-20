using Microsoft.EntityFrameworkCore;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Infrastructure.Persistence;

namespace OficinaMecanica.Infrastructure.Administrativo.Repositories;

public sealed class ServicoCatalogoRepository : IServicoCatalogoRepository
{
    private readonly OficinaMecanicaDbContext _dbContext;

    public ServicoCatalogoRepository(OficinaMecanicaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AdicionarAsync(ServicoCatalogo servico, CancellationToken cancellationToken = default)
    {
        await _dbContext.ServicosCatalogo.AddAsync(servico, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(ServicoCatalogo servico, CancellationToken cancellationToken = default)
    {
        _dbContext.ServicosCatalogo.Update(servico);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(ServicoCatalogo servico, CancellationToken cancellationToken = default)
    {
        _dbContext.ServicosCatalogo.Remove(servico);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<ServicoCatalogo?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.ServicosCatalogo
            .SingleOrDefaultAsync(servico => servico.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<ServicoCatalogo>> ObterPorIdsAsync(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ServicosCatalogo
            .Where(servico => ids.Contains(servico.Id))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ServicoCatalogo>> ListarAsync(int pagina, int tamanhoPagina, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ServicosCatalogo
            .AsNoTracking()
            .OrderBy(servico => servico.Descricao)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> ContarAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.ServicosCatalogo.CountAsync(cancellationToken);
    }
}
