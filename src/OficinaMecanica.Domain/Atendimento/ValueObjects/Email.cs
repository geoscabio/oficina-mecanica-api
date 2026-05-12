using System.Text.RegularExpressions;
using OficinaMecanica.Domain.Atendimento.Exceptions;

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
            throw new ClienteInvalidoException("E-mail invalido.");
        }

        return new Email(endereco.Trim().ToLowerInvariant());
    }

    [GeneratedRegex("^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$")]
    private static partial Regex EmailRegex();
}
