using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Atendimento.ClienteUseCases.Responses;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Application.Atendimento.ClienteUseCases.CadastrarCliente;

public sealed class CadastrarClienteUseCase
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IValidator<CadastrarClienteRequest> _validator;
    private readonly IMapper _mapper;

    public CadastrarClienteUseCase(
        IClienteRepository clienteRepository,
        IValidator<CadastrarClienteRequest> validator,
        IMapper mapper)
    {
        _clienteRepository = clienteRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<ClienteResponse>> ExecuteAsync(
        CadastrarClienteRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<ClienteResponse>.Falha(validationResult.ObterMensagensErro(), TipoErro.Validacao);
        }

        var documento = CpfCnpj.Criar(request.Documento);
        var clienteExistente = await _clienteRepository.ObterPorDocumentoAsync(documento.Numero, cancellationToken);

        if (clienteExistente is not null)
        {
            return Result<ClienteResponse>.Falha(ClienteErrorMessages.ClienteDuplicado, TipoErro.RegraNegocio);
        }

        var cliente = Cliente.Criar(
            documento,
            request.Nome,
            new Endereco(
                request.Endereco.Logradouro,
                request.Endereco.Numero,
                request.Endereco.Bairro,
                request.Endereco.Cidade,
                request.Endereco.CEP),
            Telefone.Criar(request.Telefone),
            Email.Criar(request.Email));

        await _clienteRepository.AdicionarAsync(cliente, cancellationToken);

        return Result<ClienteResponse>.Ok(_mapper.Map<ClienteResponse>(cliente));
    }
}




