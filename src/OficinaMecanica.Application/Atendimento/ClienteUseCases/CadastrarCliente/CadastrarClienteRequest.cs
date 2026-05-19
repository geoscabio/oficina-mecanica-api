using OficinaMecanica.Application.Atendimento.Common;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;

public sealed record CadastrarClienteRequest(
    string Documento,
    string Nome,
    EnderecoRequest Endereco,
    string Telefone,
    string Email);
