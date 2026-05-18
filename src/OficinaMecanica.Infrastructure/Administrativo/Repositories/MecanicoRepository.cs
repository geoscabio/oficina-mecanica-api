using Microsoft.EntityFrameworkCore;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Infrastructure.Persistence;

namespace OficinaMecanica.Infrastructure.Administrativo.Repositories;

public sealed class MecanicoRepository : IMecanicoRepository
{
    private readonly OficinaMecanicaDbContext _dbContext;

    public MecanicoRepository(OficinaMecanicaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AdicionarAsync(Mecanico mecanico, CancellationToken cancellationToken = default)
    {
        await _dbContext.Mecanicos.AddAsync(mecanico, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Mecanico?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Mecanicos.SingleOrDefaultAsync(mecanico => mecanico.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Mecanico>> ListarAsync(
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Mecanicos
            .AsNoTracking()
            .OrderBy(mecanico => mecanico.Nome)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Mecanico mecanico, CancellationToken cancellationToken = default)
    {
        _dbContext.Mecanicos.Update(mecanico);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(Mecanico mecanico, CancellationToken cancellationToken = default)
    {
        _dbContext.Mecanicos.Remove(mecanico);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<int> ContarAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Mecanicos.CountAsync(cancellationToken);
    }
}
