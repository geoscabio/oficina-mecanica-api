namespace OficinaMecanica.Application.Atendimento.Responses;

public sealed record ClienteResponse(
    Guid Id,
    string Documento,
    string Nome,
    EnderecoResponse Endereco,
    string Telefone,
    string Email);
