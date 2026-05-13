using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.Responses;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.ConsultarClientePorDocumento;

public sealed class ConsultarClientePorDocumentoUseCase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IValidator<ConsultarClientePorDocumentoRequest> _validator;
    private readonly IMapper _mapper;

    public ConsultarClientePorDocumentoUseCase(
        IClienteRepository clienteRepository,
        IValidator<ConsultarClientePorDocumentoRequest> validator,
        IMapper mapper)
    {
        _clienteRepository = clienteRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<ClienteResponse>> ExecuteAsync(
        ConsultarClientePorDocumentoRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<ClienteResponse>.Falha(validationResult.Errors.First().ErrorMessage, TipoErro.Validacao);
        }

        var documento = CpfCnpj.Criar(request.Documento);
        var cliente = await _clienteRepository.ObterPorDocumentoAsync(documento.Numero, cancellationToken);

        if (cliente is null)
        {
            return Result<ClienteResponse>.Falha(ClienteErrorMessages.ClienteNaoEncontrado, TipoErro.NaoEncontrado);
        }

        return Result<ClienteResponse>.Ok(_mapper.Map<ClienteResponse>(cliente));
    }
}



