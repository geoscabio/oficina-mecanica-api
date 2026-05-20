using Microsoft.EntityFrameworkCore;
using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Entities;
using OficinaMecanica.Infrastructure.Administrativo.Seed;
using OficinaMecanica.Infrastructure.Persistence;

namespace OficinaMecanica.Infrastructure.GestaoEstoque.Seed;

internal static class EstoqueSeedData
{
    private const int FiltroOleoQuantidadeDisponivel = 20;
    private const int PastilhaFreioQuantidadeDisponivel = 12;
    private const int OleoMotorQuantidadeDisponivel = 30;

    public static async Task<EstoqueSeedResult> SeedAsync(OficinaMecanicaDbContext dbContext, AdministrativoSeedResult administrativo, CancellationToken cancellationToken)
    {
        var estoque = await ObterOuCriarAsync(
            dbContext,
            new[]
            {
                new ItemEstoqueSeed(administrativo.PecaFiltroOleo.Id, FiltroOleoQuantidadeDisponivel),
                new ItemEstoqueSeed(administrativo.PecaPastilhaFreio.Id, PastilhaFreioQuantidadeDisponivel),
                new ItemEstoqueSeed(administrativo.InsumoOleoMotor.Id, OleoMotorQuantidadeDisponivel)
            },
            cancellationToken);

        return new EstoqueSeedResult(estoque);
    }

    private static async Task<Estoque> ObterOuCriarAsync(OficinaMecanicaDbContext dbContext, IReadOnlyCollection<ItemEstoqueSeed> itensSeed, CancellationToken cancellationToken)
    {
        var estoqueExistente = await dbContext.Estoques
            .Include(estoque => estoque.ItensEstoque)
            .SingleOrDefaultAsync(cancellationToken);

        if (estoqueExistente is not null)
        {
            return estoqueExistente;
        }

        var itensEstoque = itensSeed
            .Select(item => ItemEstoque.Criar(item.PecaInsumoCatalogoId, item.QuantidadeDisponivel))
            .ToList();

        var estoque = Estoque.Criar(itensEstoque);
        dbContext.Estoques.Add(estoque);

        return estoque;
    }

    private sealed record ItemEstoqueSeed(Guid PecaInsumoCatalogoId, int QuantidadeDisponivel);
}

internal sealed record EstoqueSeedResult(Estoque Estoque);
