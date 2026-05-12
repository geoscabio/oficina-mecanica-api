namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;

public sealed record CadastrarClienteResponse(
    Guid Id,
    string Documento,
    string Nome,
    string Email);
