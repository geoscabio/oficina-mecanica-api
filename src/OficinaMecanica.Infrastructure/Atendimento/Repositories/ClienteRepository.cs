using Microsoft.EntityFrameworkCore;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Infrastructure.Persistence;

namespace OficinaMecanica.Infrastructure.Atendimento.Repositories;

public sealed class ClienteRepository : IClienteRepository
{
    private readonly OficinaMecanicaDbContext _dbContext;

    public ClienteRepository(OficinaMecanicaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AdicionarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        await _dbContext.Clientes.AddAsync(cliente, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Cliente?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Clientes
                         .SingleOrDefaultAsync(cliente => cliente.Id == id, cancellationToken);
    }

    public Task<Cliente?> ObterPorDocumentoAsync(string documento, CancellationToken cancellationToken = default)
    {
        return _dbContext.Clientes
                         .SingleOrDefaultAsync(cliente => cliente.Documento.Numero == documento, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Cliente>> ListarAsync(int pagina, int tamanhoPagina, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Clientes
            .AsNoTracking()
            .OrderBy(cliente => cliente.Nome)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        _dbContext.Clientes.Update(cliente);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(Cliente cliente, CancellationToken cancellationToken = default)
    {
        _dbContext.Clientes.Remove(cliente);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<int> ContarAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Clientes.CountAsync(cancellationToken);
    }
}
