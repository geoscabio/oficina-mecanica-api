using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Enums;

namespace OficinaMecanica.Domain.UnitTests.Administrativo.Builders;

internal static class PecaInsumoCatalogoTestDataFactory
{
    public const string DescricaoPadrao = "Filtro de oleo";
    public const decimal ValorPadrao = 45m;

    public static PecaInsumoCatalogo CriarPecaInsumoCatalogoPadrao(
        TipoPecaInsumo tipo = TipoPecaInsumo.PECA)
    {
        return PecaInsumoCatalogo.Criar(DescricaoPadrao, tipo, ValorPadrao);
    }
}
