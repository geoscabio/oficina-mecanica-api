using OficinaMecanica.Domain.GestaoOrdemServico.Exceptions;

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
            throw new PecaInsumoInvalidaException("Peca ou insumo do catalogo e obrigatorio.");
        }

        if (quantidade <= 0)
        {
            throw new PecaInsumoInvalidaException("Quantidade da peca ou insumo deve ser maior que zero.");
        }

        if (valorUnitario <= 0)
        {
            throw new PecaInsumoInvalidaException("Valor unitario da peca ou insumo deve ser maior que zero.");
        }

        return new PecaInsumo(Guid.NewGuid(), pecaInsumoCatalogoId, quantidade, valorUnitario);
    }
}
