using Microsoft.EntityFrameworkCore;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Infrastructure.Persistence;

namespace OficinaMecanica.Infrastructure.Administrativo.Seed;

internal static class MecanicoSeedData
{
    private static readonly MecanicoSeed MecanicoPrincipal = new("Joao Silva", "MEC001");
    private static readonly MecanicoSeed MecanicoDiagnostico = new("Mariana Costa", "MEC002");

    public static Task<Mecanico> ObterOuCriarPrincipalAsync(
        OficinaMecanicaDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return ObterOuCriarAsync(dbContext, MecanicoPrincipal, cancellationToken);
    }

    public static Task<Mecanico> ObterOuCriarDiagnosticoAsync(
        OficinaMecanicaDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return ObterOuCriarAsync(dbContext, MecanicoDiagnostico, cancellationToken);
    }

    private static async Task<Mecanico> ObterOuCriarAsync(
        OficinaMecanicaDbContext dbContext,
        MecanicoSeed seed,
        CancellationToken cancellationToken)
    {
        var mecanicoExistente = await dbContext.Mecanicos
            .SingleOrDefaultAsync(mecanico => mecanico.Funcional == seed.Funcional, cancellationToken);

        if (mecanicoExistente is not null)
        {
            return mecanicoExistente;
        }

        var mecanico = Mecanico.Criar(seed.Nome, seed.Funcional);
        dbContext.Mecanicos.Add(mecanico);

        return mecanico;
    }

    private sealed record MecanicoSeed(string Nome, string Funcional);
}
