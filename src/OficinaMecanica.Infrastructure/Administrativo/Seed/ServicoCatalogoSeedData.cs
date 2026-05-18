using Microsoft.EntityFrameworkCore;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Infrastructure.Persistence;

namespace OficinaMecanica.Infrastructure.Administrativo.Seed;

internal static class ServicoCatalogoSeedData
{
    private static readonly ServicoSeed ServicoTrocaOleo = new("Troca de oleo", 120m);
    private static readonly ServicoSeed ServicoAlinhamento = new("Alinhamento", 180m);
    private static readonly ServicoSeed ServicoDiagnostico = new("Diagnostico tecnico", 150m);

    public static Task<ServicoCatalogo> ObterOuCriarTrocaOleoAsync(
        OficinaMecanicaDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return ObterOuCriarAsync(dbContext, ServicoTrocaOleo, cancellationToken);
    }

    public static Task<ServicoCatalogo> ObterOuCriarAlinhamentoAsync(
        OficinaMecanicaDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return ObterOuCriarAsync(dbContext, ServicoAlinhamento, cancellationToken);
    }

    public static Task<ServicoCatalogo> ObterOuCriarDiagnosticoAsync(
        OficinaMecanicaDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return ObterOuCriarAsync(dbContext, ServicoDiagnostico, cancellationToken);
    }

    private static async Task<ServicoCatalogo> ObterOuCriarAsync(
        OficinaMecanicaDbContext dbContext,
        ServicoSeed seed,
        CancellationToken cancellationToken)
    {
        var servicoExistente = await dbContext.ServicosCatalogo
            .SingleOrDefaultAsync(servico => servico.Descricao == seed.Descricao, cancellationToken);

        if (servicoExistente is not null)
        {
            return servicoExistente;
        }

        var servico = ServicoCatalogo.Criar(seed.Descricao, seed.Valor);
        dbContext.ServicosCatalogo.Add(servico);

        return servico;
    }

    private sealed record ServicoSeed(string Descricao, decimal Valor);
}
