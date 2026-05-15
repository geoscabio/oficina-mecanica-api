using OficinaMecanica.Domain.Administrativo.Enums;
using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Factories;

internal static class PecaInsumoCatalogoTestDataFactory
{
    public static PecaInsumoCatalogo CriarPecaInsumoCatalogoPadrao(
        string descricao = "Filtro de oleo",
        TipoPecaInsumo tipo = TipoPecaInsumo.PECA,
        decimal valor = 45m)
    {
        return PecaInsumoCatalogo.Criar(descricao, tipo, valor);
    }
}

