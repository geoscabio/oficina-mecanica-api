using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;

namespace OficinaMecanica.Domain.GestaoOrdemServico.Entities;

public sealed class PecaInsumo
{
    private PecaInsumo(Guid id, Guid pecaInsumoCatalogoId, int quantidade, decimal valorUnitario)
    {
        Id = id;
        PecaInsumoCatalogoId = pecaInsumoCatalogoId;
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
    }

    public Guid Id { get; private set; }
    public Guid PecaInsumoCatalogoId { get; private set; }
    public int Quantidade { get; private set; }
    public decimal ValorUnitario { get; private set; }
    public decimal ValorTotal => Quantidade * ValorUnitario;

    public static PecaInsumo Criar(Guid pecaInsumoCatalogoId, int quantidade, decimal valorUnitario)
    {
        if (pecaInsumoCatalogoId == Guid.Empty)
        {
            throw new DomainException(OrdemServicoErrorMessages.PecaInsumoCatalogoObrigatorio);
        }

        if (quantidade <= 0)
        {
            throw new DomainException(OrdemServicoErrorMessages.QuantidadePecaInsumoMaiorQueZero);
        }

        if (valorUnitario <= 0)
        {
            throw new DomainException(OrdemServicoErrorMessages.ValorUnitarioPecaInsumoMaiorQueZero);
        }

        return new PecaInsumo(Guid.NewGuid(), pecaInsumoCatalogoId, quantidade, valorUnitario);
    }
}

