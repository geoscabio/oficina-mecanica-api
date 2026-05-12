using OficinaMecanica.Domain.Administrativo.Enums;
using OficinaMecanica.Domain.Administrativo.Exceptions;

namespace OficinaMecanica.Domain.Administrativo.Aggregates;

public sealed class PecaInsumoCatalogo
{
    private PecaInsumoCatalogo(Guid id, string descricao, TipoPecaInsumo tipo, decimal valor)
    {
        Id = id;
        Descricao = descricao;
        Tipo = tipo;
        Valor = valor;
    }

    public Guid Id { get; private set; }
    public string Descricao { get; private set; }
    public TipoPecaInsumo Tipo { get; private set; }
    public decimal Valor { get; private set; }

    public static PecaInsumoCatalogo Criar(string descricao, TipoPecaInsumo tipo, decimal valor)
    {
        if (string.IsNullOrWhiteSpace(descricao))
        {
            throw new PecaInsumoCatalogoInvalidaException("Descricao da peca ou insumo e obrigatoria.");
        }

        if (!Enum.IsDefined(tipo))
        {
            throw new PecaInsumoCatalogoInvalidaException("Tipo da peca ou insumo e invalido.");
        }

        if (valor <= 0)
        {
            throw new PecaInsumoCatalogoInvalidaException("Valor da peca ou insumo deve ser maior que zero.");
        }

        return new PecaInsumoCatalogo(Guid.NewGuid(), descricao.Trim(), tipo, valor);
    }
}
