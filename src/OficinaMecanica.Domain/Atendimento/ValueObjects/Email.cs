using System.Text.RegularExpressions;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.Atendimento.Messages;

namespace OficinaMecanica.Domain.Atendimento.ValueObjects;

public sealed partial record Email
{
    private Email(string endereco)
    {
        Endereco = endereco;
    }

    public string Endereco { get; }

    public static Email Criar(string endereco)
    {
        if (string.IsNullOrWhiteSpace(endereco) || !EmailRegex().IsMatch(endereco.Trim()))
        {
            throw new DomainException(ClienteErrorMessages.EmailInvalido);
        }

        return new Email(endereco.Trim().ToLowerInvariant());
    }

    [GeneratedRegex("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$")]
    private static partial Regex EmailRegex();
}

