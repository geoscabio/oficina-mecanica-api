using OficinaMecanica.Domain.Administrativo.Aggregates;

namespace OficinaMecanica.Domain.UnitTests.Administrativo.Factories;

internal static class ServicoCatalogoTestDataFactory
{
    public const string DescricaoPadrao = "Troca de oleo";
    public const decimal ValorPadrao = 120m;

    public const string DescricaoAtualizada = "Alinhamento";
    public const decimal ValorAtualizado = 90m;

    public static ServicoCatalogo CriarServicoCatalogoPadrao(string descricao = DescricaoPadrao, decimal valor = ValorPadrao)
    {
        return ServicoCatalogo.Criar(descricao, valor);
    }
}