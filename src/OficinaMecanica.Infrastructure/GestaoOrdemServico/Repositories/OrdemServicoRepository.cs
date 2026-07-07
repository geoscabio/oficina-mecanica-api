using Microsoft.EntityFrameworkCore;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Entities;
using OficinaMecanica.Domain.GestaoOrdemServico.Enums;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;
using OficinaMecanica.Infrastructure.Persistence;

namespace OficinaMecanica.Infrastructure.GestaoOrdemServico.Repositories;

public sealed class OrdemServicoRepository : IOrdemServicoRepository
{
    private readonly OficinaMecanicaDbContext _dbContext;

    public OrdemServicoRepository(OficinaMecanicaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AdicionarAsync(OrdemServico ordemServico, CancellationToken cancellationToken = default)
    {
        await _dbContext.OrdensServico.AddAsync(ordemServico, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(OrdemServico ordemServico, CancellationToken cancellationToken = default)
    {
        _dbContext.OrdensServico.Update(ordemServico);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<OrdemServico?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.OrdensServico
            .Include(ordemServico => ordemServico.Servicos)
            .Include(ordemServico => ordemServico.PecasInsumos)
            .AsSplitQuery()
            .SingleOrDefaultAsync(ordemServico => ordemServico.Id == id, cancellationToken);
    }

    public async Task<int> ObterProximoNumeroAsync(CancellationToken cancellationToken = default)
    {
        var ultimoNumero = await _dbContext.OrdensServico
            .Select(ordemServico => (int?)ordemServico.Numero)
            .MaxAsync(cancellationToken);

        return (ultimoNumero ?? 0) + 1;
    }

    public async Task<IReadOnlyCollection<OrdemServico>> ListarAsync(int pagina, int tamanhoPagina, CancellationToken cancellationToken = default)
    {
        return await ObterOrdensServicoListaveis()
            .AsNoTracking()
            .Include(ordemServico => ordemServico.Servicos)
            .Include(ordemServico => ordemServico.PecasInsumos)
            .AsSplitQuery()
            .OrderBy(ordemServico => ordemServico.Status == StatusOrdemServico.EmExecucao ? 0
                : ordemServico.Status == StatusOrdemServico.AguardandoAprovacao ? 1
                : ordemServico.Status == StatusOrdemServico.EmDiagnostico ? 2
                : 3)
            .ThenBy(ordemServico => ordemServico.DataInicio)
            .ThenBy(ordemServico => ordemServico.Numero)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> ContarAsync(CancellationToken cancellationToken = default)
    {
        return ObterOrdensServicoListaveis().CountAsync(cancellationToken);
    }

    public Task<double?> ObterTempoMedioExecucaoServicoAsync(Guid servicoCatalogoId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Set<Servico>()
            .Where(servico => servico.ServicoCatalogoId == servicoCatalogoId
                && servico.Status == StatusServico.Finalizado
                && servico.DataInicio.HasValue
                && servico.DataFim.HasValue)
            .Select(servico => (double?)EF.Functions.DateDiffMinute(servico.DataInicio!.Value, servico.DataFim!.Value))
            .AverageAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<Guid, double>> ListarTemposMediosExecucaoServicosAsync(IReadOnlyCollection<Guid> servicosCatalogoIds, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Servico>()
            .Where(servico => servicosCatalogoIds.Contains(servico.ServicoCatalogoId)
                && servico.Status == StatusServico.Finalizado
                && servico.DataInicio.HasValue
                && servico.DataFim.HasValue)
            .GroupBy(servico => servico.ServicoCatalogoId)
            .Select(grupo => new
            {
                ServicoCatalogoId = grupo.Key,
                TempoMedio = grupo.Average(servico => (double?)EF.Functions.DateDiffMinute(servico.DataInicio!.Value, servico.DataFim!.Value))
            })
            .Where(item => item.TempoMedio.HasValue)
            .ToDictionaryAsync(item => item.ServicoCatalogoId, item => item.TempoMedio!.Value, cancellationToken);
    }

    private IQueryable<OrdemServico> ObterOrdensServicoListaveis()
    {
        return _dbContext.OrdensServico
            .Where(ordemServico => ordemServico.Status == StatusOrdemServico.EmExecucao
                || ordemServico.Status == StatusOrdemServico.AguardandoAprovacao
                || ordemServico.Status == StatusOrdemServico.EmDiagnostico
                || ordemServico.Status == StatusOrdemServico.Recebida);
    }
}

