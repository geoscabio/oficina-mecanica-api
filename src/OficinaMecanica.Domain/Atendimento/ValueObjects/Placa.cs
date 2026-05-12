using System.Text.RegularExpressions;
using OficinaMecanica.Domain.Atendimento.Exceptions;

namespace OficinaMecanica.Domain.Atendimento.ValueObjects;

public sealed partial record Placa
{
    private Placa(string numeroPlaca)
    {
        NumeroPlaca = numeroPlaca;
    }

    public string NumeroPlaca { get; }

    public static Placa Criar(string numeroPlaca)
    {
        var numeroNormalizado = Normalizar(numeroPlaca);

        if (!PlacaMercosulRegex().IsMatch(numeroNormalizado)
            && !PlacaAntigaRegex().IsMatch(numeroNormalizado))
        {
            throw new PlacaInvalidaException("Placa invalida.");
        }

        return new Placa(numeroNormalizado);
    }

    private static string Normalizar(string numeroPlaca)
    {
        if (string.IsNullOrWhiteSpace(numeroPlaca))
        {
            return string.Empty;
        }

        return numeroPlaca.Trim().Replace("-", string.Empty).Replace(" ", string.Empty).ToUpperInvariant();
    }

    [GeneratedRegex("^[A-Z]{3}[0-9][A-Z][0-9]{2}$")]
    private static partial Regex PlacaMercosulRegex();

    [GeneratedRegex("^[A-Z]{3}[0-9]{4}$")]
    private static partial Regex PlacaAntigaRegex();
}
