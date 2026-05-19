using OficinaMecanica.Application.Atendimento.Common;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.AtualizarCliente;

public sealed record AtualizarClienteRequest(
    Guid ClienteId,
    string Nome,
    EnderecoRequest Endereco,
    string Telefone,
    string Email);
