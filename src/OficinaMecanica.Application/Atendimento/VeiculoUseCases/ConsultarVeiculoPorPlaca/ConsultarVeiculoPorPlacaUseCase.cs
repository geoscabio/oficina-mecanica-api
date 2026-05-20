using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.Responses;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculoPorPlaca;

public sealed class ConsultarVeiculoPorPlacaUseCase
{
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IValidator<ConsultarVeiculoPorPlacaRequest> _validator;
    private readonly IMapper _mapper;

    public ConsultarVeiculoPorPlacaUseCase(
        IVeiculoRepository veiculoRepository,
        IValidator<ConsultarVeiculoPorPlacaRequest> validator,
        IMapper mapper)
    {
        _veiculoRepository = veiculoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<VeiculoResponse>> ExecuteAsync(
        ConsultarVeiculoPorPlacaRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<VeiculoResponse>.Falha(validationResult.ObterMensagensErro(), TipoErro.Validacao);
        }

        var placa = Placa.Criar(request.Placa);
        var veiculo = await _veiculoRepository.ObterPorPlacaAsync(placa.NumeroPlaca, cancellationToken);

        if (veiculo is null)
        {
            return Result<VeiculoResponse>.Falha(VeiculoErrorMessages.VeiculoNaoEncontrado, TipoErro.NaoEncontrado);
        }

        return Result<VeiculoResponse>.Ok(_mapper.Map<VeiculoResponse>(veiculo));
    }
}





