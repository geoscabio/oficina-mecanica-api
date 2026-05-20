using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Infrastructure.Persistence;

namespace OficinaMecanica.Infrastructure.Atendimento.Seed;

internal static class AtendimentoSeedData
{
    public static async Task<AtendimentoSeedResult> SeedAsync(OficinaMecanicaDbContext dbContext, CancellationToken cancellationToken)
    {
        var maria = await ClienteSeedData.ObterOuCriarMariaAsync(dbContext, cancellationToken);
        var carlos = await ClienteSeedData.ObterOuCriarCarlosAsync(dbContext, cancellationToken);

        var civic = await VeiculoSeedData.ObterOuCriarCivicAsync(dbContext, maria.Id, cancellationToken);
        var onix = await VeiculoSeedData.ObterOuCriarOnixAsync(dbContext, maria.Id, cancellationToken);
        var corolla = await VeiculoSeedData.ObterOuCriarCorollaAsync(dbContext, carlos.Id, cancellationToken);

        return new AtendimentoSeedResult(maria, carlos, civic, onix, corolla);
    }
}

internal sealed record AtendimentoSeedResult(Cliente ClienteMaria, Cliente ClienteCarlos, Veiculo VeiculoCivic, Veiculo VeiculoOnix, Veiculo VeiculoCorolla);
