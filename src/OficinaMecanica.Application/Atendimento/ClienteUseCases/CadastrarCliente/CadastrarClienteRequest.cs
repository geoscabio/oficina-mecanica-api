namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;

public sealed record CadastrarClienteRequest(
    string Documento,
    string Nome,
    string Logradouro,
    string Numero,
    string Bairro,
    string Cidade,
    string CEP,
    string Telefone,
    string Email);
