using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.Responses;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Atendimento.Interfaces;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculo;

public sealed class ConsultarVeiculoUseCase
{
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IValidator<ConsultarVeiculoRequest> _validator;
    private readonly IMapper _mapper;

    public ConsultarVeiculoUseCase(
        IVeiculoRepository veiculoRepository,
        IValidator<ConsultarVeiculoRequest> validator,
        IMapper mapper)
    {
        _veiculoRepository = veiculoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<VeiculoResponse>> ExecuteAsync(
        ConsultarVeiculoRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<VeiculoResponse>.Falha(validationResult.ObterMensagensErro(), TipoErro.Validacao);
        }

        var veiculo = await _veiculoRepository.ObterPorIdAsync(request.Id, cancellationToken);

        if (veiculo is null)
        {
            return Result<VeiculoResponse>.Falha(VeiculoErrorMessages.VeiculoNaoEncontrado, TipoErro.NaoEncontrado);
        }

        return Result<VeiculoResponse>.Ok(_mapper.Map<VeiculoResponse>(veiculo));
    }
}





