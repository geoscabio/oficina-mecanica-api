using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.AtualizarEstoque;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.ConsultarItemEstoque;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.ListarItensEstoque;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.RegistrarEntradaEstoque;
using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Entities;

namespace OficinaMecanica.Application.UnitTests.GestaoEstoque.Factories;

internal static class EstoqueTestDataFactory
{
    public const int QuantidadeDisponivelPadrao = 10;
    public const int QuantidadeEntradaPadrao = 5;
    public const int QuantidadeAtualizadaPadrao = 3;
    public const int PaginaPadrao = 1;
    public const int TamanhoPaginaPadrao = 10;

    public static ItemEstoque CriarItemEstoquePadrao(Guid? pecaInsumoCatalogoId = null, int quantidadeDisponivel = QuantidadeDisponivelPadrao)
    {
        return ItemEstoque.Criar(pecaInsumoCatalogoId ?? Guid.NewGuid(), quantidadeDisponivel);
    }

    public static Estoque CriarEstoquePadrao(Guid? pecaInsumoCatalogoId = null, int quantidadeDisponivel = QuantidadeDisponivelPadrao)
    {
        return Estoque.Criar(new[]
        {
            CriarItemEstoquePadrao(pecaInsumoCatalogoId, quantidadeDisponivel)
        });
    }

    public static Estoque CriarEstoqueComItens(int quantidadeItens)
    {
        var itens = Enumerable
            .Range(1, quantidadeItens)
            .Select(_ => CriarItemEstoquePadrao())
            .ToArray();

        return Estoque.Criar(itens);
    }

    public static AtualizarEstoqueRequest CriarAtualizarEstoqueRequestValido(Guid? pecaInsumoCatalogoId = null, int quantidadeDisponivel = QuantidadeAtualizadaPadrao)
    {
        return new AtualizarEstoqueRequest(pecaInsumoCatalogoId ?? Guid.NewGuid(), quantidadeDisponivel);
    }

    public static ConsultarItemEstoqueRequest CriarConsultarItemEstoqueRequestValido(Guid? itemEstoqueId = null)
    {
        return new ConsultarItemEstoqueRequest(itemEstoqueId ?? Guid.NewGuid());
    }

    public static ListarItensEstoqueRequest CriarListarItensEstoqueRequestValido(int pagina = PaginaPadrao, int tamanhoPagina = TamanhoPaginaPadrao)
    {
        return new ListarItensEstoqueRequest(pagina, tamanhoPagina);
    }

    public static RegistrarEntradaEstoqueRequest CriarRegistrarEntradaEstoqueRequestValido(Guid? pecaInsumoCatalogoId = null, int quantidade = QuantidadeEntradaPadrao)
    {
        return new RegistrarEntradaEstoqueRequest(pecaInsumoCatalogoId ?? Guid.NewGuid(), quantidade);
    }
}