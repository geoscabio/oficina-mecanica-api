using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Infrastructure.Persistence;

namespace OficinaMecanica.Infrastructure.Administrativo.Seed;

internal static class AdministrativoSeedData
{
    public static async Task<AdministrativoSeedResult> SeedAsync(OficinaMecanicaDbContext dbContext, CancellationToken cancellationToken)
    {
        var mecanicoPrincipal = await MecanicoSeedData.ObterOuCriarPrincipalAsync(dbContext, cancellationToken);
        var mecanicoDiagnostico = await MecanicoSeedData.ObterOuCriarDiagnosticoAsync(dbContext, cancellationToken);

        var trocaOleo = await ServicoCatalogoSeedData.ObterOuCriarTrocaOleoAsync(dbContext, cancellationToken);
        var alinhamento = await ServicoCatalogoSeedData.ObterOuCriarAlinhamentoAsync(dbContext, cancellationToken);
        var diagnostico = await ServicoCatalogoSeedData.ObterOuCriarDiagnosticoAsync(dbContext, cancellationToken);

        var filtroOleo = await PecaInsumoCatalogoSeedData.ObterOuCriarFiltroOleoAsync(dbContext, cancellationToken);
        var pastilhaFreio = await PecaInsumoCatalogoSeedData.ObterOuCriarPastilhaFreioAsync(dbContext, cancellationToken);
        var oleoMotor = await PecaInsumoCatalogoSeedData.ObterOuCriarOleoMotorAsync(dbContext, cancellationToken);

        return new AdministrativoSeedResult(mecanicoPrincipal, mecanicoDiagnostico, trocaOleo, alinhamento, diagnostico, filtroOleo, pastilhaFreio, oleoMotor);
    }
}

internal sealed record AdministrativoSeedResult(
    Mecanico MecanicoPrincipal,
    Mecanico MecanicoDiagnostico,
    ServicoCatalogo ServicoTrocaOleo,
    ServicoCatalogo ServicoAlinhamento,
    ServicoCatalogo ServicoDiagnostico,
    PecaInsumoCatalogo PecaFiltroOleo,
    PecaInsumoCatalogo PecaPastilhaFreio,
    PecaInsumoCatalogo InsumoOleoMotor);
