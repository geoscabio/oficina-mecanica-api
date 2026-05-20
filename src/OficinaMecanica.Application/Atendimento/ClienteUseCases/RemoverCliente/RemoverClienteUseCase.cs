using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.Messages;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.RemoverCliente;

public sealed class RemoverClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IValidator<RemoverClienteRequest> _validator;
    private readonly IMapper _mapper;

    public RemoverClienteUseCase(
        IClienteRepository clienteRepository,
        IValidator<RemoverClienteRequest> validator,
        IMapper mapper)
    {
        _clienteRepository = clienteRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<ClienteResponse>> ExecuteAsync(
        RemoverClienteRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<ClienteResponse>.Falha(
                validationResult.ObterMensagensErro(),
                TipoErro.Validacao);
        }

        var cliente = await _clienteRepository.ObterPorIdAsync(
            request.ClienteId,
            cancellationToken);

        if (cliente is null)
        {
            return Result<ClienteResponse>.Falha(
                ClienteErrorMessages.ClienteNaoEncontrado,
                TipoErro.NaoEncontrado);
        }

        await _clienteRepository.RemoverAsync(cliente, cancellationToken);

        return Result<ClienteResponse>.Ok(_mapper.Map<ClienteResponse>(cliente));
    }
}
