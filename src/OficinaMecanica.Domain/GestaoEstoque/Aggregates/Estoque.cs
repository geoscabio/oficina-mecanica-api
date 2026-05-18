using OficinaMecanica.Domain.GestaoEstoque.Entities;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.GestaoEstoque.Messages;

namespace OficinaMecanica.Domain.GestaoEstoque.Aggregates;

public sealed class Estoque
{
    private readonly List<ItemEstoque> _itensEstoque;

    private Estoque()
    {
        _itensEstoque = new List<ItemEstoque>();
    }

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
            throw new DomainException(EstoqueErrorMessages.EstoqueSemItens);
        }

        if (itens.Any(item => item is null))
        {
            throw new DomainException(EstoqueErrorMessages.EstoqueComItemNulo);
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

    public ItemEstoque RegistrarEntrada(Guid pecaInsumoCatalogoId, int quantidade)
    {
        var item = EncontrarItem(pecaInsumoCatalogoId);

        if (item is null)
        {
            item = ItemEstoque.Criar(pecaInsumoCatalogoId, quantidade);
            _itensEstoque.Add(item);

            return item;
        }

        item.RegistrarEntrada(quantidade);

        return item;
    }

    public ItemEstoque AtualizarQuantidadeDisponivel(Guid pecaInsumoCatalogoId, int quantidadeDisponivel)
    {
        var item = ObterItem(pecaInsumoCatalogoId);

        item.AtualizarQuantidadeDisponivel(quantidadeDisponivel);

        return item;
    }

    public ItemEstoque ObterItem(Guid pecaInsumoCatalogoId)
    {
        return EncontrarItem(pecaInsumoCatalogoId)
            ?? throw new DomainException(EstoqueErrorMessages.ItemNaoEncontrado);
    }

    private ItemEstoque? EncontrarItem(Guid pecaInsumoCatalogoId)
    {
        if (pecaInsumoCatalogoId == Guid.Empty)
        {
            throw new DomainException(EstoqueErrorMessages.PecaInsumoCatalogoObrigatorio);
        }

        return _itensEstoque.SingleOrDefault(item => item.PecaInsumoCatalogoId == pecaInsumoCatalogoId);
    }

    public ItemEstoque? ObterItemPorPecaInsumoCatalogoId(Guid pecaInsumoCatalogoId)
    {
        return _itensEstoque.FirstOrDefault(
            item => item.PecaInsumoCatalogoId == pecaInsumoCatalogoId);
    }
}

