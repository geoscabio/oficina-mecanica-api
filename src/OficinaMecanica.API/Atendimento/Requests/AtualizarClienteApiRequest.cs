using OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;

namespace OficinaMecanica.API.Atendimento.Requests;

public sealed record AtualizarClienteApiRequest(
    string Nome,
    EnderecoRequest Endereco,
    string Telefone,
    string Email);
