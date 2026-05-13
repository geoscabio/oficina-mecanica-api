using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Atendimento.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Atendimento.Interfaces;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarCliente;

public sealed class ConsultarClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IValidator<ConsultarClienteRequest> _validator;
    private readonly IMapper _mapper;

    public ConsultarClienteUseCase(
        IClienteRepository clienteRepository,
        IValidator<ConsultarClienteRequest> validator,
        IMapper mapper)
    {
        _clienteRepository = clienteRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<ClienteResponse>> ExecuteAsync(
        ConsultarClienteRequest request,
        CancellationToken cancellationToken = default)
    {
        _validator.ValidateAndThrow(request);

        var cliente = await _clienteRepository.ObterPorIdAsync(request.Id, cancellationToken);

        if (cliente is null)
        {
            return Result<ClienteResponse>.Falha("Cliente nao encontrado.");
        }

        return Result<ClienteResponse>.Ok(_mapper.Map<ClienteResponse>(cliente));
    }
}
