using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Entities;

namespace OficinaMecanica.Domain.UnitTests.GestaoEstoque.Builders;

internal static class EstoqueTestDataFactory
{
    public const int QuantidadeDisponivelPadrao = 10;

    public static ItemEstoque CriarItemEstoquePadrao(
        Guid? pecaInsumoCatalogoId = null,
        int quantidadeDisponivel = QuantidadeDisponivelPadrao)
    {
        return ItemEstoque.Criar(
            pecaInsumoCatalogoId ?? Guid.NewGuid(),
            quantidadeDisponivel);
    }

    public static Estoque CriarEstoquePadrao(
        Guid? pecaInsumoCatalogoId = null,
        int quantidadeDisponivel = QuantidadeDisponivelPadrao)
    {
        var item = CriarItemEstoquePadrao(pecaInsumoCatalogoId, quantidadeDisponivel);

        return Estoque.Criar(new[] { item });
    }
}
