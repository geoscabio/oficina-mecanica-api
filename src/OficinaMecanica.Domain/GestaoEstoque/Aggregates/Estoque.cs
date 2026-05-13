using OficinaMecanica.Domain.GestaoEstoque.Entities;
using OficinaMecanica.Domain.GestaoEstoque.Exceptions;

namespace OficinaMecanica.Domain.GestaoEstoque.Aggregates;

public sealed class Estoque
{
    private readonly List<ItemEstoque> _itensEstoque;

    private Estoque(Guid id, IEnumerable<ItemEstoque> itensEstoque)
    {
        Id = id;
        _itensEstoque = itensEstoque.ToList();
    }

    public Guid Id { get; private set; }
    public IReadOnlyCollection<ItemEstoque> ItensEstoque => _itensEstoque.AsReadOnly();

    public static Estoque Criar(IEnumerable<ItemEstoque> itensEstoque)
    {
        var itens = itensEstoque?.ToList() ?? new List<ItemEstoque>();

        if (itens.Count == 0)
        {
            throw new EstoqueInvalidoException("Estoque deve possuir ao menos um item.");
        }

        if (itens.Any(item => item is null))
        {
            throw new EstoqueInvalidoException("Estoque nao pode possuir item nulo.");
        }

        return new Estoque(Guid.NewGuid(), itens);
    }

    public bool VerificarDisponibilidade(Guid pecaInsumoCatalogoId, int quantidade)
    {
        var item = EncontrarItem(pecaInsumoCatalogoId);

        return item is not null && item.PossuiDisponibilidade(quantidade);
    }

    public void ReservarItens(Guid pecaInsumoCatalogoId, int quantidade)
    {
        ObterItem(pecaInsumoCatalogoId).Reservar(quantidade);
    }

    public void EstornarItens(Guid pecaInsumoCatalogoId, int quantidade)
    {
        ObterItem(pecaInsumoCatalogoId).Estornar(quantidade);
    }

    public void BaixarItens(Guid pecaInsumoCatalogoId, int quantidade)
    {
        ObterItem(pecaInsumoCatalogoId).Baixar(quantidade);
    }

    public ItemEstoque ObterItem(Guid pecaInsumoCatalogoId)
    {
        return EncontrarItem(pecaInsumoCatalogoId)
            ?? throw new EstoqueInvalidoException("Item de estoque nao encontrado.");
    }

    private ItemEstoque? EncontrarItem(Guid pecaInsumoCatalogoId)
    {
        if (pecaInsumoCatalogoId == Guid.Empty)
        {
            throw new EstoqueInvalidoException("Peca ou insumo do catalogo e obrigatorio.");
        }

        return _itensEstoque.SingleOrDefault(item => item.PecaInsumoCatalogoId == pecaInsumoCatalogoId);
    }
}
