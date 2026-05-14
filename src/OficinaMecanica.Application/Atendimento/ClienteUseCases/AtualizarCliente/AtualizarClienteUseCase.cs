using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.AtualizarCliente;

public sealed class AtualizarClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IValidator<AtualizarClienteRequest> _validator;
    private readonly IMapper _mapper;

    public AtualizarClienteUseCase(
        IClienteRepository clienteRepository,
        IValidator<AtualizarClienteRequest> validator,
        IMapper mapper)
    {
        _clienteRepository = clienteRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<ClienteResponse>> ExecuteAsync(
        AtualizarClienteRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<ClienteResponse>.Falha(validationResult.Errors.First().ErrorMessage, TipoErro.Validacao);
        }

        var cliente = await _clienteRepository.ObterPorIdAsync(request.ClienteId, cancellationToken);

        if (cliente is null)
        {
            return Result<ClienteResponse>.Falha(ClienteErrorMessages.ClienteNaoEncontrado, TipoErro.NaoEncontrado);
        }

        cliente.Atualizar(
            request.Nome,
            new Endereco(
                request.Endereco.Logradouro,
                request.Endereco.Numero,
                request.Endereco.Bairro,
                request.Endereco.Cidade,
                request.Endereco.CEP),
            Telefone.Criar(request.Telefone),
            Email.Criar(request.Email));

        await _clienteRepository.AtualizarAsync(cliente, cancellationToken);

        return Result<ClienteResponse>.Ok(_mapper.Map<ClienteResponse>(cliente));
    }
}