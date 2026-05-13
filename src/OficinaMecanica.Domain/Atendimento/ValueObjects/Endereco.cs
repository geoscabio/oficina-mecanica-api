using OficinaMecanica.Domain.Atendimento.Exceptions;
using OficinaMecanica.Domain.Atendimento.Messages;

namespace OficinaMecanica.Domain.Atendimento.ValueObjects;

public sealed record Endereco
{
    public Endereco(string logradouro, string numero, string bairro, string cidade, string cep)
    {
        if (string.IsNullOrWhiteSpace(logradouro)
            || string.IsNullOrWhiteSpace(numero)
            || string.IsNullOrWhiteSpace(bairro)
            || string.IsNullOrWhiteSpace(cidade)
            || string.IsNullOrWhiteSpace(cep))
        {
            throw new ClienteInvalidoException(ClienteErrorMessages.EnderecoInvalido);
        }

        Logradouro = logradouro.Trim();
        Numero = numero.Trim();
        Bairro = bairro.Trim();
        Cidade = cidade.Trim();
        CEP = NormalizarCep(cep);
    }

    public string Logradouro { get; }
    public string Numero { get; }
    public string Bairro { get; }
    public string Cidade { get; }
    public string CEP { get; }

    private static string NormalizarCep(string cep)
    {
        var cepNormalizado = new string(cep.Where(char.IsDigit).ToArray());

        if (cepNormalizado.Length != 8)
        {
            throw new ClienteInvalidoException(ClienteErrorMessages.CepInvalido);
        }

        return cepNormalizado;
    }
}

