using Microsoft.EntityFrameworkCore;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Enums;
using OficinaMecanica.Infrastructure.Persistence;

namespace OficinaMecanica.Infrastructure.Administrativo.Seed;

internal static class PecaInsumoCatalogoSeedData
{
    private static readonly PecaInsumoSeed PecaFiltroOleo = new("Filtro de oleo", TipoPecaInsumo.PECA, 45m);
    private static readonly PecaInsumoSeed PecaPastilhaFreio = new("Pastilha de freio", TipoPecaInsumo.PECA, 220m);
    private static readonly PecaInsumoSeed InsumoOleoMotor = new("Oleo de motor 5W30", TipoPecaInsumo.INSUMO, 65m);

    public static Task<PecaInsumoCatalogo> ObterOuCriarFiltroOleoAsync(
        OficinaMecanicaDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return ObterOuCriarAsync(dbContext, PecaFiltroOleo, cancellationToken);
    }

    public static Task<PecaInsumoCatalogo> ObterOuCriarPastilhaFreioAsync(
        OficinaMecanicaDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return ObterOuCriarAsync(dbContext, PecaPastilhaFreio, cancellationToken);
    }

    public static Task<PecaInsumoCatalogo> ObterOuCriarOleoMotorAsync(
        OficinaMecanicaDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return ObterOuCriarAsync(dbContext, InsumoOleoMotor, cancellationToken);
    }

    private static async Task<PecaInsumoCatalogo> ObterOuCriarAsync(
        OficinaMecanicaDbContext dbContext,
        PecaInsumoSeed seed,
        CancellationToken cancellationToken)
    {
        var pecaInsumoExistente = await dbContext.PecasInsumosCatalogo
            .SingleOrDefaultAsync(pecaInsumo => pecaInsumo.Descricao == seed.Descricao, cancellationToken);

        if (pecaInsumoExistente is not null)
        {
            return pecaInsumoExistente;
        }

        var pecaInsumo = PecaInsumoCatalogo.Criar(seed.Descricao, seed.Tipo, seed.Valor);
        dbContext.PecasInsumosCatalogo.Add(pecaInsumo);

        return pecaInsumo;
    }

    private sealed record PecaInsumoSeed(string Descricao, TipoPecaInsumo Tipo, decimal Valor);
}
