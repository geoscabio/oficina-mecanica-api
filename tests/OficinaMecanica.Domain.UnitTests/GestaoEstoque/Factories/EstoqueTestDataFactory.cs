using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Entities;

namespace OficinaMecanica.Domain.UnitTests.GestaoEstoque.Factories;

internal static class EstoqueTestDataFactory
{
    public const int QuantidadeDisponivelPadrao = 10;
    public const int QuantidadeReservadaPadrao = 4;
    public const int QuantidadeEntradaPadrao = 5;
    public const int QuantidadeAtualizadaPadrao = 3;
    public const int QuantidadeIndisponivelPadrao = 15;

    public static ItemEstoque CriarItemEstoquePadrao(Guid? pecaInsumoCatalogoId = null, int quantidadeDisponivel = QuantidadeDisponivelPadrao)
    {
        return ItemEstoque.Criar(pecaInsumoCatalogoId ?? Guid.NewGuid(), quantidadeDisponivel);
    }

    public static ItemEstoque CriarItemEstoqueComReserva(Guid? pecaInsumoCatalogoId = null, int quantidadeDisponivel = QuantidadeDisponivelPadrao, int quantidadeReservada = QuantidadeReservadaPadrao)
    {
        var item = CriarItemEstoquePadrao(pecaInsumoCatalogoId, quantidadeDisponivel);

        item.Reservar(quantidadeReservada);

        return item;
    }

    public static Estoque CriarEstoquePadrao(Guid? pecaInsumoCatalogoId = null, int quantidadeDisponivel = QuantidadeDisponivelPadrao)
    {
        return Estoque.Criar(new[]
        {
            CriarItemEstoquePadrao(pecaInsumoCatalogoId, quantidadeDisponivel)
        });
    }

    public static Estoque CriarEstoqueComItemReservado(Guid? pecaInsumoCatalogoId = null, int quantidadeDisponivel = QuantidadeDisponivelPadrao, int quantidadeReservada = QuantidadeReservadaPadrao)
    {
        return Estoque.Criar(new[]
        {
            CriarItemEstoqueComReserva(pecaInsumoCatalogoId, quantidadeDisponivel, quantidadeReservada)
        });
    }
}