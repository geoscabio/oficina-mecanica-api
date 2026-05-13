using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Entities;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Builders;

internal static class EstoqueTestDataFactory
{
    public static Estoque CriarEstoqueComItem(Guid pecaInsumoCatalogoId, int quantidadeDisponivel = 10)
    {
        return Estoque.Criar(new[] { ItemEstoque.Criar(pecaInsumoCatalogoId, quantidadeDisponivel) });
    }
}

