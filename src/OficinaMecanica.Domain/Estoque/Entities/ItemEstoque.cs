using OficinaMecanica.Domain.Estoque.Exceptions;

namespace OficinaMecanica.Domain.Estoque.Entities;

public sealed class ItemEstoque
{
    private ItemEstoque(Guid id, Guid pecaInsumoCatalogoId, int quantidadeDisponivel, int quantidadeReservada)
    {
        Id = id;
        PecaInsumoCatalogoId = pecaInsumoCatalogoId;
        QuantidadeDisponivel = quantidadeDisponivel;
        QuantidadeReservada = quantidadeReservada;
    }

    public Guid Id { get; private set; }
    public Guid PecaInsumoCatalogoId { get; private set; }
    public int QuantidadeDisponivel { get; private set; }
    public int QuantidadeReservada { get; private set; }

    public static ItemEstoque Criar(Guid pecaInsumoCatalogoId, int quantidadeDisponivel)
    {
        if (pecaInsumoCatalogoId == Guid.Empty)
        {
            throw new ItemEstoqueInvalidoException("Peca ou insumo do catalogo e obrigatorio.");
        }

        if (quantidadeDisponivel < 0)
        {
            throw new ItemEstoqueInvalidoException("Quantidade disponivel nao pode ser negativa.");
        }

        return new ItemEstoque(Guid.NewGuid(), pecaInsumoCatalogoId, quantidadeDisponivel, 0);
    }

    public bool PossuiDisponibilidade(int quantidade)
    {
        ValidarQuantidadePositiva(quantidade);

        return QuantidadeDisponivel >= quantidade;
    }

    public void Reservar(int quantidade)
    {
        ValidarQuantidadePositiva(quantidade);

        if (!PossuiDisponibilidade(quantidade))
        {
            throw new EstoqueInsuficienteException("Estoque insuficiente para reservar peca ou insumo.");
        }

        QuantidadeDisponivel -= quantidade;
        QuantidadeReservada += quantidade;
    }

    public void Estornar(int quantidade)
    {
        ValidarQuantidadePositiva(quantidade);
        ValidarQuantidadeReservadaSuficiente(quantidade);

        QuantidadeReservada -= quantidade;
        QuantidadeDisponivel += quantidade;
    }

    public void Baixar(int quantidade)
    {
        ValidarQuantidadePositiva(quantidade);
        ValidarQuantidadeReservadaSuficiente(quantidade);

        QuantidadeReservada -= quantidade;
    }

    private static void ValidarQuantidadePositiva(int quantidade)
    {
        if (quantidade <= 0)
        {
            throw new ItemEstoqueInvalidoException("Quantidade deve ser maior que zero.");
        }
    }

    private void ValidarQuantidadeReservadaSuficiente(int quantidade)
    {
        if (QuantidadeReservada < quantidade)
        {
            throw new ItemEstoqueInvalidoException("Quantidade reservada insuficiente.");
        }
    }
}
