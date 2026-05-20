using OficinaMecanica.Domain.Shared.Exceptions;
using OficinaMecanica.Domain.Atendimento.Messages;

namespace OficinaMecanica.Domain.Atendimento.ValueObjects;

public sealed record Endereco
{
    private Endereco(string logradouro, string numero, string bairro, string cidade, string CEP)
    {
        if (string.IsNullOrWhiteSpace(logradouro)
            || string.IsNullOrWhiteSpace(numero)
            || string.IsNullOrWhiteSpace(bairro)
            || string.IsNullOrWhiteSpace(cidade)
            || string.IsNullOrWhiteSpace(CEP))
        {
            throw new DomainException(ClienteErrorMessages.EnderecoInvalido);
        }

        Logradouro = logradouro.Trim();
        Numero = numero.Trim();
        Bairro = bairro.Trim();
        Cidade = cidade.Trim();
        this.CEP = NormalizarCep(CEP);
    }

    public string Logradouro { get; }
    public string Numero { get; }
    public string Bairro { get; }
    public string Cidade { get; }
    public string CEP { get; }

    public static Endereco Criar(string logradouro, string numero, string bairro, string cidade, string CEP)
    {
        return new Endereco(logradouro, numero, bairro, cidade, CEP);
    }

    private static string NormalizarCep(string cep)
    {
        var cepNormalizado = new string(cep.Where(char.IsDigit).ToArray());

        if (cepNormalizado.Length != 8)
        {
            throw new DomainException(ClienteErrorMessages.CepInvalido);
        }

        return cepNormalizado;
    }
}

