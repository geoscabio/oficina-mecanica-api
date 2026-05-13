namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.Responses;

public sealed record ClienteResponse(
    Guid Id,
    string Documento,
    string Nome,
    EnderecoResponse Endereco,
    string Telefone,
    string Email);
