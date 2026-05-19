using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.AtualizarEstoque;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.RegistrarEntradaEstoque;

namespace OficinaMecanica.API.IntegrationTests.GestaoEstoque.Builders;

public sealed class EstoqueRequestBuilder
{
    private Guid _pecaInsumoCatalogoId = Guid.NewGuid();
    private int _quantidade = 5;

    public static EstoqueRequestBuilder Novo()
    {
        return new EstoqueRequestBuilder();
    }

    public EstoqueRequestBuilder ComPecaInsumoCatalogoId(Guid pecaInsumoCatalogoId)
    {
        _pecaInsumoCatalogoId = pecaInsumoCatalogoId;

        return this;
    }

    public EstoqueRequestBuilder ComQuantidade(int quantidade)
    {
        _quantidade = quantidade;

        return this;
    }

    public RegistrarEntradaEstoqueRequest BuildRegistroEntrada()
    {
        return new RegistrarEntradaEstoqueRequest(_pecaInsumoCatalogoId, _quantidade);
    }

    public AtualizarEstoqueRequest BuildAtualizacao()
    {
        return new AtualizarEstoqueRequest(_pecaInsumoCatalogoId, _quantidade);
    }
}
