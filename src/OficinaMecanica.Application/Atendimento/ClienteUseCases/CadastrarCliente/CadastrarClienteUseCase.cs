using AutoMapper;
using FluentValidation;
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

    public async Task<Result<CadastrarClienteResponse>> ExecuteAsync(
        CadastrarClienteRequest request,
        CancellationToken cancellationToken = default)
    {
        _validator.ValidateAndThrow(request);

        var documento = CpfCnpj.Criar(request.Documento);
        var clienteExistente = await _clienteRepository.ObterPorDocumentoAsync(documento.Numero, cancellationToken);

        if (clienteExistente is not null)
        {
            return Result<CadastrarClienteResponse>.Falha("Cliente ja cadastrado para o documento informado.");
        }

        var cliente = Cliente.Criar(
            documento,
            request.Nome,
            new Endereco(request.Logradouro, request.Numero, request.Bairro, request.Cidade, request.CEP),
            Telefone.Criar(request.Telefone),
            Email.Criar(request.Email));

        await _clienteRepository.AdicionarAsync(cliente, cancellationToken);

        return Result<CadastrarClienteResponse>.Ok(_mapper.Map<CadastrarClienteResponse>(cliente));
    }
}
