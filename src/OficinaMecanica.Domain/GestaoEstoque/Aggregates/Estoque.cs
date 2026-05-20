using OficinaMecanica.Domain.GestaoEstoque.Entities;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.GestaoEstoque.Messages;
using OficinaMecanica.Domain.Shared.Results;

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

    public ResultadoDominio ReservarItens(Guid pecaInsumoCatalogoId, int quantidade)
    {
        var resultadoItem = ObterItemParaOperacao(pecaInsumoCatalogoId);

        return resultadoItem.Sucesso
            ? resultadoItem.Valor!.Reservar(quantidade)
            : ResultadoDominio.Falha(resultadoItem.Mensagem!);
    }

    public ResultadoDominio EstornarItens(Guid pecaInsumoCatalogoId, int quantidade)
    {
        var resultadoItem = ObterItemParaOperacao(pecaInsumoCatalogoId);

        return resultadoItem.Sucesso
            ? resultadoItem.Valor!.Estornar(quantidade)
            : ResultadoDominio.Falha(resultadoItem.Mensagem!);
    }

    public ResultadoDominio BaixarItens(Guid pecaInsumoCatalogoId, int quantidade)
    {
        var resultadoItem = ObterItemParaOperacao(pecaInsumoCatalogoId);

        return resultadoItem.Sucesso
            ? resultadoItem.Valor!.Baixar(quantidade)
            : ResultadoDominio.Falha(resultadoItem.Mensagem!);
    }

    public ResultadoDominio<ItemEstoque> RegistrarEntrada(Guid pecaInsumoCatalogoId, int quantidade)
    {
        if (pecaInsumoCatalogoId == Guid.Empty)
        {
            return ResultadoDominio<ItemEstoque>.Falha(EstoqueErrorMessages.PecaInsumoCatalogoObrigatorio);
        }

        if (quantidade <= 0)
        {
            return ResultadoDominio<ItemEstoque>.Falha(EstoqueErrorMessages.QuantidadeMaiorQueZero);
        }

        var item = EncontrarItem(pecaInsumoCatalogoId);

        if (item is null)
        {
            item = ItemEstoque.Criar(pecaInsumoCatalogoId, quantidade);
            _itensEstoque.Add(item);

            return ResultadoDominio<ItemEstoque>.Ok(item);
        }

        var resultadoEntrada = item.RegistrarEntrada(quantidade);

        return resultadoEntrada.Sucesso
            ? ResultadoDominio<ItemEstoque>.Ok(item)
            : ResultadoDominio<ItemEstoque>.Falha(resultadoEntrada.Mensagem!);
    }

    public ResultadoDominio<ItemEstoque> AtualizarQuantidadeDisponivel(
        Guid pecaInsumoCatalogoId,
        int quantidadeDisponivel)
    {
        var resultadoItem = ObterItemParaOperacao(pecaInsumoCatalogoId);
        if (!resultadoItem.Sucesso)
        {
            return ResultadoDominio<ItemEstoque>.Falha(resultadoItem.Mensagem!);
        }

        var item = resultadoItem.Valor!;

        var resultadoAtualizacao = item.AtualizarQuantidadeDisponivel(quantidadeDisponivel);

        return resultadoAtualizacao.Sucesso
            ? ResultadoDominio<ItemEstoque>.Ok(item)
            : ResultadoDominio<ItemEstoque>.Falha(resultadoAtualizacao.Mensagem!);
    }

    public ItemEstoque ObterItem(Guid pecaInsumoCatalogoId)
    {
        return EncontrarItem(pecaInsumoCatalogoId)
            ?? throw new DomainException(EstoqueErrorMessages.ItemNaoEncontrado);
    }

    private ResultadoDominio<ItemEstoque> ObterItemParaOperacao(Guid pecaInsumoCatalogoId)
    {
        if (pecaInsumoCatalogoId == Guid.Empty)
        {
            return ResultadoDominio<ItemEstoque>.Falha(EstoqueErrorMessages.PecaInsumoCatalogoObrigatorio);
        }

        var item = EncontrarItem(pecaInsumoCatalogoId);

        return item is null
            ? ResultadoDominio<ItemEstoque>.Falha(EstoqueErrorMessages.ItemNaoEncontrado)
            : ResultadoDominio<ItemEstoque>.Ok(item);
    }

    private ItemEstoque? EncontrarItem(Guid pecaInsumoCatalogoId)
    {
        if (pecaInsumoCatalogoId == Guid.Empty)
        {
            throw new DomainException(EstoqueErrorMessages.PecaInsumoCatalogoObrigatorio);
        }

        return _itensEstoque.SingleOrDefault(item => item.PecaInsumoCatalogoId == pecaInsumoCatalogoId);
    }
}
