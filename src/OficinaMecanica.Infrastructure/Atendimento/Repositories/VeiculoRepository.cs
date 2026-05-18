using Microsoft.EntityFrameworkCore;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Infrastructure.Persistence;

namespace OficinaMecanica.Infrastructure.Atendimento.Repositories;

public sealed class VeiculoRepository : IVeiculoRepository
{
    private readonly OficinaMecanicaDbContext _dbContext;

    public VeiculoRepository(OficinaMecanicaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AdicionarAsync(Veiculo veiculo, CancellationToken cancellationToken = default)
    {
        await _dbContext.Veiculos.AddAsync(veiculo, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Veiculo?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Veiculos
            .SingleOrDefaultAsync(veiculo => veiculo.Id == id, cancellationToken);
    }

    public Task<Veiculo?> ObterPorPlacaAsync(string placa, CancellationToken cancellationToken = default)
    {
        return _dbContext.Veiculos
            .SingleOrDefaultAsync(veiculo => veiculo.Placa.NumeroPlaca == placa, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Veiculo>> ListarAsync(
        int pagina,
        int tamanhoPagina,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Veiculos
            .AsNoTracking()
            .OrderBy(veiculo => veiculo.Placa.NumeroPlaca)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AtualizarAsync(Veiculo veiculo, CancellationToken cancellationToken = default)
    {
        _dbContext.Veiculos.Update(veiculo);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(Veiculo veiculo, CancellationToken cancellationToken = default)
    {
        _dbContext.Veiculos.Remove(veiculo);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<int> ContarAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Veiculos.CountAsync(cancellationToken);
    }
}
