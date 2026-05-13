using OficinaMecanica.Domain.Administrativo.Enums;
using OficinaMecanica.Domain.Administrativo.Exceptions;
using OficinaMecanica.Domain.Administrativo.Messages;

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
            throw new PecaInsumoCatalogoInvalidaException(PecaInsumoCatalogoErrorMessages.DescricaoObrigatoria);
        }

        if (!Enum.IsDefined(tipo))
        {
            throw new PecaInsumoCatalogoInvalidaException(PecaInsumoCatalogoErrorMessages.TipoInvalido);
        }

        if (valor <= 0)
        {
            throw new PecaInsumoCatalogoInvalidaException(PecaInsumoCatalogoErrorMessages.ValorMaiorQueZero);
        }

        return new PecaInsumoCatalogo(Guid.NewGuid(), descricao.Trim(), tipo, valor);
    }
}

