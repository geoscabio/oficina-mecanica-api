using OficinaMecanica.Domain.Atendimento.Enums;
using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.Atendimento.Messages;

namespace OficinaMecanica.Domain.Atendimento.ValueObjects;

public sealed record CpfCnpj
{
    private static readonly int[] PrimeiroDigitoCnpjPesos = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
    private static readonly int[] SegundoDigitoCnpjPesos = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

    private CpfCnpj(string numero, TipoDocumento tipo)
    {
        Numero = numero;
        Tipo = tipo;
    }

    public string Numero { get; }
    public TipoDocumento Tipo { get; }

    public static CpfCnpj Criar(string numero)
    {
        var numeroNormalizado = Normalizar(numero);

        return numeroNormalizado.Length switch
        {
            11 when CpfValido(numeroNormalizado) => new CpfCnpj(numeroNormalizado, TipoDocumento.CPF),
            14 when CnpjValido(numeroNormalizado) => new CpfCnpj(numeroNormalizado, TipoDocumento.CNPJ),
            _ => throw new DomainException(ClienteErrorMessages.DocumentoInvalido)
        };
    }

    private static string Normalizar(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
        {
            return string.Empty;
        }

        return new string(numero.Where(char.IsDigit).ToArray());
    }

    private static bool CpfValido(string numero)
    {
        if (TodosDigitosIguais(numero))
        {
            return false;
        }

        var primeiroDigito = CalcularDigito(numero[..9], 10);
        var segundoDigito = CalcularDigito(numero[..10], 11);

        return numero[9] == DigitoParaChar(primeiroDigito)
            && numero[10] == DigitoParaChar(segundoDigito);
    }

    private static bool CnpjValido(string numero)
    {
        if (TodosDigitosIguais(numero))
        {
            return false;
        }

        var primeiroDigito = CalcularDigitoCnpj(numero[..12], PrimeiroDigitoCnpjPesos);
        var segundoDigito = CalcularDigitoCnpj(numero[..13], SegundoDigitoCnpjPesos);

        return numero[12] == DigitoParaChar(primeiroDigito)
            && numero[13] == DigitoParaChar(segundoDigito);
    }

    private static int CalcularDigito(string baseNumero, int pesoInicial)
    {
        var soma = 0;

        for (var i = 0; i < baseNumero.Length; i++)
        {
            soma += (baseNumero[i] - '0') * (pesoInicial - i);
        }

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    private static int CalcularDigitoCnpj(string baseNumero, int[] pesos)
    {
        var soma = 0;

        for (var i = 0; i < baseNumero.Length; i++)
        {
            soma += (baseNumero[i] - '0') * pesos[i];
        }

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    private static bool TodosDigitosIguais(string numero)
    {
        return numero.All(digito => digito == numero[0]);
    }

    private static char DigitoParaChar(int digito)
    {
        return (char)('0' + digito);
    }
}

