namespace OficinaMecanica.Application.Atendimento.Responses;

public sealed record EnderecoResponse(
    string Logradouro,
    string Numero,
    string Bairro,
    string Cidade,
    string CEP);
