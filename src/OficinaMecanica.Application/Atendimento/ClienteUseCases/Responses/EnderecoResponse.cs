namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.Responses;

public sealed record EnderecoResponse(
    string Logradouro,
    string Numero,
    string Bairro,
    string Cidade,
    string CEP);
