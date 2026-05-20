using OficinaMecanica.Domain.Administrativo.Enums;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.Administrativo.Messages;

namespace OficinaMecanica.Domain.Administrativo.Aggregates;

public sealed class PecaInsumoCatalogo
{
    private PecaInsumoCatalogo()
    {
        Descricao = string.Empty;
    }

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
            throw new DomainException(PecaInsumoCatalogoErrorMessages.DescricaoObrigatoria);
        }

        if (!Enum.IsDefined(tipo))
        {
            throw new DomainException(PecaInsumoCatalogoErrorMessages.TipoInvalido);
        }

        if (valor <= 0)
        {
            throw new DomainException(PecaInsumoCatalogoErrorMessages.ValorMaiorQueZero);
        }

        return new PecaInsumoCatalogo(Guid.NewGuid(), descricao.Trim(), tipo, valor);
    }

    public void Atualizar(string descricao, TipoPecaInsumo tipo, decimal valor)
    {
        if (string.IsNullOrWhiteSpace(descricao))
        {
            throw new DomainException(PecaInsumoCatalogoErrorMessages.DescricaoObrigatoria);
        }

        if (!Enum.IsDefined(tipo))
        {
            throw new DomainException(PecaInsumoCatalogoErrorMessages.TipoInvalido);
        }

        if (valor <= 0)
        {
            throw new DomainException(PecaInsumoCatalogoErrorMessages.ValorMaiorQueZero);
        }

        Descricao = descricao.Trim();
        Tipo = tipo;
        Valor = valor;
    }
}

