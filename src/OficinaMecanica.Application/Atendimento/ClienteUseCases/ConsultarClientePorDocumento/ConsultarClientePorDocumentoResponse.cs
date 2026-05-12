namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarClientePorDocumento;

public sealed record ConsultarClientePorDocumentoResponse(
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
