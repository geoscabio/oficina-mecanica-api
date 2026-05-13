namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;

public sealed record CadastrarClienteRequest(
    string Documento,
    string Nome,
    EnderecoRequest Endereco,
    string Telefone,
    string Email);

public sealed record EnderecoRequest(
    string Logradouro,
    string Numero,
    string Bairro,
    string Cidade,
    string CEP);
