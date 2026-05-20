using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.Responses;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Atendimento.Interfaces;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarCliente;

public sealed class ConsultarClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IValidator<ConsultarClienteRequest> _validator;
    private readonly IMapper _mapper;

    public ConsultarClienteUseCase(IClienteRepository clienteRepository, IValidator<ConsultarClienteRequest> validator, IMapper mapper)
    {
        _clienteRepository = clienteRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<ClienteResponse>> ExecuteAsync(ConsultarClienteRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<ClienteResponse>.Falha(validationResult.ObterMensagensErro(), TipoErro.Validacao);
        }

        var cliente = await _clienteRepository.ObterPorIdAsync(request.Id, cancellationToken);

        if (cliente is null)
        {
            return Result<ClienteResponse>.Falha(ClienteErrorMessages.ClienteNaoEncontrado, TipoErro.NaoEncontrado);
        }

        return Result<ClienteResponse>.Ok(_mapper.Map<ClienteResponse>(cliente));
    }
}




