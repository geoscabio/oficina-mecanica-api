namespace OficinaMecanica.Application.Atendimento.Common;

public sealed record EnderecoRequest(string Logradouro, string Numero, string Bairro, string Cidade, string CEP);
