using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Domain.UnitTests.Administrativo.Builders;

internal static class ServicoCatalogoTestDataFactory
{
    public const string DescricaoPadrao = "Troca de oleo";
    public const decimal ValorPadrao = 120m;

    public static ServicoCatalogo CriarServicoCatalogoPadrao()
    {
        return ServicoCatalogo.Criar(DescricaoPadrao, ValorPadrao);
    }
}
