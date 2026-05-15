using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Enums;

namespace OficinaMecanica.Domain.UnitTests.Administrativo.Factories;

internal static class PecaInsumoCatalogoTestDataFactory
{
    public const string DescricaoPadrao = "Filtro de oleo";
    public const TipoPecaInsumo TipoPadrao = TipoPecaInsumo.PECA;
    public const decimal ValorPadrao = 45m;

    public const string DescricaoAtualizada = "Oleo 5W30";
    public const TipoPecaInsumo TipoAtualizado = TipoPecaInsumo.INSUMO;
    public const decimal ValorAtualizado = 38m;

    public static PecaInsumoCatalogo CriarPecaInsumoCatalogoPadrao(
        string descricao = DescricaoPadrao,
        TipoPecaInsumo tipo = TipoPadrao,
        decimal valor = ValorPadrao)
    {
        return PecaInsumoCatalogo.Criar(
            descricao,
            tipo,
            valor);
    }
}