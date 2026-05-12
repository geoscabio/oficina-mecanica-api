using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarCliente;

public sealed class ConsultarClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly ConsultarClienteValidator _validator;

    public ConsultarClienteUseCase(IClienteRepository clienteRepository, ConsultarClienteValidator validator)
    {
        _clienteRepository = clienteRepository;
        _validator = validator;
    }

    public async Task<Result<ConsultarClienteResponse>> ExecuteAsync(
        ConsultarClienteRequest request,
        CancellationToken cancellationToken = default)
    {
        _validator.Validate(request);

        var cliente = await _clienteRepository.ObterPorIdAsync(request.Id, cancellationToken);

        if (cliente is null)
        {
            return Result<ConsultarClienteResponse>.Falha("Cliente nao encontrado.");
        }

        return Result<ConsultarClienteResponse>.Ok(MapearResponse(cliente));
    }

    private static ConsultarClienteResponse MapearResponse(Cliente cliente)
    {
        return new ConsultarClienteResponse(
            cliente.Id,
            cliente.Documento.Numero,
            cliente.Nome,
            cliente.Endereco.Logradouro,
            cliente.Endereco.Numero,
            cliente.Endereco.Bairro,
            cliente.Endereco.Cidade,
            cliente.Endereco.CEP,
            cliente.Telefone.Numero,
            cliente.Email.Endereco);
    }
}
