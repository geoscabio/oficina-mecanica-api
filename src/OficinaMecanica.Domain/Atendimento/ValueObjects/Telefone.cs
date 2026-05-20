using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.Atendimento.Messages;

namespace OficinaMecanica.Domain.Atendimento.ValueObjects;

public sealed record Telefone
{
    private Telefone(string numero)
    {
        Numero = numero;
    }

    public string Numero { get; }

    public static Telefone Criar(string numero)
    {
        var numeroNormalizado = string.IsNullOrWhiteSpace(numero)
            ? string.Empty
            : new string(numero.Where(char.IsDigit).ToArray());

        if (numeroNormalizado.Length is < 10 or > 11)
        {
            throw new DomainException(ClienteErrorMessages.TelefoneInvalido);
        }

        return new Telefone(numeroNormalizado);
    }
}

