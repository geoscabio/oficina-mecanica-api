using OficinaMecanica.Infrastructure.Administrativo.Seed;
using OficinaMecanica.Infrastructure.Atendimento.Seed;
using OficinaMecanica.Infrastructure.GestaoEstoque.Seed;
using OficinaMecanica.Infrastructure.GestaoOrdemServico.Seed;

namespace OficinaMecanica.Infrastructure.Persistence;

internal static class DemoDatabaseSeeder
{
    public static async Task SeedAsync(OficinaMecanicaDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var administrativo = await AdministrativoSeedData.SeedAsync(dbContext, cancellationToken);
        var estoque = await EstoqueSeedData.SeedAsync(dbContext, administrativo, cancellationToken);
        var atendimento = await AtendimentoSeedData.SeedAsync(dbContext, cancellationToken);

        await OrdemServicoSeedData.SeedAsync(dbContext, administrativo, atendimento, estoque, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
