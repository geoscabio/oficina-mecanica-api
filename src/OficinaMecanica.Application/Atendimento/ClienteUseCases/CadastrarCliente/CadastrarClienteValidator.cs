using OficinaMecanica.Application.Common.Exceptions;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;

public sealed class CadastrarClienteValidator
{
    public void Validate(CadastrarClienteRequest request)
    {
        if (request is null)
        {
            throw new ValidationException("Request de cadastro de cliente e obrigatorio.");
        }

        ValidarObrigatorio(request.Documento, "Documento");
        ValidarObrigatorio(request.Nome, "Nome");
        ValidarObrigatorio(request.Logradouro, "Logradouro");
        ValidarObrigatorio(request.Numero, "Numero");
        ValidarObrigatorio(request.Bairro, "Bairro");
        ValidarObrigatorio(request.Cidade, "Cidade");
        ValidarObrigatorio(request.CEP, "CEP");
        ValidarObrigatorio(request.Telefone, "Telefone");
        ValidarObrigatorio(request.Email, "Email");
    }

    private static void ValidarObrigatorio(string valor, string campo)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new ValidationException($"{campo} e obrigatorio.");
        }
    }
}
