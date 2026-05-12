namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarCliente;

public sealed record ConsultarClienteResponse(
    Guid Id,
    string Documento,
    string Nome,
    string Logradouro,
    string Numero,
    string Bairro,
    string Cidade,
    string CEP,
    string Telefone,
    string Email);
