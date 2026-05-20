using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.GestaoEstoque.Messages;
using OficinaMecanica.Domain.Shared.Results;

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
        var resultadoQuantidade = ValidarQuantidadePositiva(quantidade);
        if (!resultadoQuantidade.Sucesso)
        {
            return false;
        }

        return QuantidadeDisponivel >= quantidade;
    }

    public ResultadoDominio Reservar(int quantidade)
    {
        var resultadoQuantidade = ValidarQuantidadePositiva(quantidade);
        if (!resultadoQuantidade.Sucesso)
        {
            return resultadoQuantidade;
        }

        if (QuantidadeDisponivel < quantidade)
        {
            return ResultadoDominio.Falha(EstoqueErrorMessages.EstoqueInsuficiente);
        }

        QuantidadeDisponivel -= quantidade;
        QuantidadeReservada += quantidade;

        return ResultadoDominio.Ok();
    }

    public ResultadoDominio Estornar(int quantidade)
    {
        var resultadoQuantidade = ValidarQuantidadePositiva(quantidade);
        if (!resultadoQuantidade.Sucesso)
        {
            return resultadoQuantidade;
        }

        var resultadoReserva = ValidarQuantidadeReservadaSuficiente(quantidade);
        if (!resultadoReserva.Sucesso)
        {
            return resultadoReserva;
        }

        QuantidadeReservada -= quantidade;
        QuantidadeDisponivel += quantidade;

        return ResultadoDominio.Ok();
    }

    public ResultadoDominio Baixar(int quantidade)
    {
        var resultadoQuantidade = ValidarQuantidadePositiva(quantidade);
        if (!resultadoQuantidade.Sucesso)
        {
            return resultadoQuantidade;
        }

        var resultadoReserva = ValidarQuantidadeReservadaSuficiente(quantidade);
        if (!resultadoReserva.Sucesso)
        {
            return resultadoReserva;
        }

        QuantidadeReservada -= quantidade;

        return ResultadoDominio.Ok();
    }

    public ResultadoDominio RegistrarEntrada(int quantidade)
    {
        var resultadoQuantidade = ValidarQuantidadePositiva(quantidade);
        if (!resultadoQuantidade.Sucesso)
        {
            return resultadoQuantidade;
        }

        QuantidadeDisponivel += quantidade;

        return ResultadoDominio.Ok();
    }

    public ResultadoDominio AtualizarQuantidadeDisponivel(int quantidadeDisponivel)
    {
        if (quantidadeDisponivel < 0)
        {
            return ResultadoDominio.Falha(EstoqueErrorMessages.QuantidadeDisponivelNaoNegativa);
        }

        QuantidadeDisponivel = quantidadeDisponivel;

        return ResultadoDominio.Ok();
    }

    private static ResultadoDominio ValidarQuantidadePositiva(int quantidade)
    {
        if (quantidade <= 0)
        {
            return ResultadoDominio.Falha(EstoqueErrorMessages.QuantidadeMaiorQueZero);
        }

        return ResultadoDominio.Ok();
    }

    private ResultadoDominio ValidarQuantidadeReservadaSuficiente(int quantidade)
    {
        if (QuantidadeReservada < quantidade)
        {
            return ResultadoDominio.Falha(EstoqueErrorMessages.QuantidadeReservadaInsuficiente);
        }

        return ResultadoDominio.Ok();
    }
}

