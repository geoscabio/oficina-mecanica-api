using OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.AtualizarCliente;

public sealed record AtualizarClienteRequest(
    Guid ClienteId,
    string Nome,
    EnderecoRequest Endereco,
    string Telefone,
    string Email);