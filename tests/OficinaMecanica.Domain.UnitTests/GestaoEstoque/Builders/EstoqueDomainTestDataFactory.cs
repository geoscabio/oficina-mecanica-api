using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Entities;

namespace OficinaMecanica.Domain.UnitTests.GestaoEstoque.Builders;

internal static class EstoqueDomainTestDataFactory
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
        return Estoque.Criar(new[]
        {
            CriarItemEstoquePadrao(pecaInsumoCatalogoId, quantidadeDisponivel)
        });
    }
}