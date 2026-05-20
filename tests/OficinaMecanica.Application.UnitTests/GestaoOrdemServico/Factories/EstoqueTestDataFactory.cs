using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Entities;

namespace OficinaMecanica.Application.UnitTests.GestaoOrdemServico.Factories;

internal static class EstoqueTestDataFactory
{
    public static Estoque CriarEstoqueComItem(Guid pecaInsumoCatalogoId, int quantidadeDisponivel = 10)
    {
        return Estoque.Criar(new[] { ItemEstoque.Criar(pecaInsumoCatalogoId, quantidadeDisponivel) });
    }

    public static Estoque CriarEstoqueComItemReservado(Guid pecaInsumoCatalogoId, int quantidadeDisponivel = 10, int quantidadeReservada = 2)
    {
        var estoque = CriarEstoqueComItem(pecaInsumoCatalogoId, quantidadeDisponivel);

        estoque.ReservarItens(pecaInsumoCatalogoId, quantidadeReservada);

        return estoque;
    }
}

