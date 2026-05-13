using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.GestaoEstoque.Messages;

namespace OficinaMecanica.Domain.GestaoEstoque.Entities;

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
            throw new DomainException(EstoqueErrorMessages.PecaInsumoCatalogoObrigatorio);
        }

        if (quantidadeDisponivel < 0)
        {
            throw new DomainException(EstoqueErrorMessages.QuantidadeDisponivelNaoNegativa);
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
            throw new DomainException(EstoqueErrorMessages.EstoqueInsuficiente);
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
            throw new DomainException(EstoqueErrorMessages.QuantidadeMaiorQueZero);
        }
    }

    private void ValidarQuantidadeReservadaSuficiente(int quantidade)
    {
        if (QuantidadeReservada < quantidade)
        {
            throw new DomainException(EstoqueErrorMessages.QuantidadeReservadaInsuficiente);
        }
    }
}

