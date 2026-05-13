using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.Responses;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.CadastrarVeiculo;

public sealed class CadastrarVeiculoUseCase
{
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly IValidator<CadastrarVeiculoRequest> _validator;
    private readonly IMapper _mapper;

    public CadastrarVeiculoUseCase(
        IVeiculoRepository veiculoRepository,
        IClienteRepository clienteRepository,
        IValidator<CadastrarVeiculoRequest> validator,
        IMapper mapper)
    {
        _veiculoRepository = veiculoRepository;
        _clienteRepository = clienteRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<VeiculoResponse>> ExecuteAsync(
        CadastrarVeiculoRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<VeiculoResponse>.Falha(validationResult.Errors.First().ErrorMessage, TipoErro.Validacao);
        }

        var cliente = await _clienteRepository.ObterPorIdAsync(request.ClienteId, cancellationToken);

        if (cliente is null)
        {
            return Result<VeiculoResponse>.Falha(ClienteErrorMessages.ClienteNaoEncontrado, TipoErro.NaoEncontrado);
        }

        var placa = Placa.Criar(request.Placa);
        var veiculoExistente = await _veiculoRepository.ObterPorPlacaAsync(placa.NumeroPlaca, cancellationToken);

        if (veiculoExistente is not null)
        {
            return Result<VeiculoResponse>.Falha(VeiculoErrorMessages.VeiculoDuplicado, TipoErro.RegraNegocio);
        }

        var veiculo = Veiculo.Criar(request.ClienteId, placa, request.Marca, request.Modelo, request.Ano);

        await _veiculoRepository.AdicionarAsync(veiculo, cancellationToken);

        return Result<VeiculoResponse>.Ok(_mapper.Map<VeiculoResponse>(veiculo));
    }
}




