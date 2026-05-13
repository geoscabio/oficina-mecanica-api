using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Application.UnitTests.Administrativo.Builders;

internal static class ServicoCatalogoTestDataFactory
{
    public static ServicoCatalogo CriarServicoCatalogoPadrao(
        string descricao = "Troca de oleo",
        decimal valor = 150m)
    {
        return ServicoCatalogo.Criar(descricao, valor);
    }
}
