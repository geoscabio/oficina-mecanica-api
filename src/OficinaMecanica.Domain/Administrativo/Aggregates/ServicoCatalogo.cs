using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.Administrativo.Messages;

namespace OficinaMecanica.Domain.Administrativo.Aggregates;

public sealed class ServicoCatalogo
{
    private ServicoCatalogo(Guid id, string descricao, decimal valor)
    {
        Id = id;
        Descricao = descricao;
        Valor = valor;
    }

    public Guid Id { get; private set; }
    public string Descricao { get; private set; }
    public decimal Valor { get; private set; }

    public static ServicoCatalogo Criar(string descricao, decimal valor)
    {
        if (string.IsNullOrWhiteSpace(descricao))
        {
            throw new DomainException(ServicoCatalogoErrorMessages.DescricaoObrigatoria);
        }

        if (valor <= 0)
        {
            throw new DomainException(ServicoCatalogoErrorMessages.ValorMaiorQueZero);
        }

        return new ServicoCatalogo(Guid.NewGuid(), descricao.Trim(), valor);
    }
}

