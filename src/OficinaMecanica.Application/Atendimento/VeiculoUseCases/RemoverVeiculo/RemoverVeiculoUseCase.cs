using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.Messages;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.RemoverVeiculo;

public sealed class RemoverVeiculoUseCase
{
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IValidator<RemoverVeiculoRequest> _validator;
    private readonly IMapper _mapper;

    public RemoverVeiculoUseCase(
        IVeiculoRepository veiculoRepository,
        IValidator<RemoverVeiculoRequest> validator,
        IMapper mapper)
    {
        _veiculoRepository = veiculoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<VeiculoResponse>> ExecuteAsync(
        RemoverVeiculoRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<VeiculoResponse>.Falha(
                validationResult.ObterMensagensErro(),
                TipoErro.Validacao);
        }

        var veiculo = await _veiculoRepository.ObterPorIdAsync(
            request.VeiculoId,
            cancellationToken);

        if (veiculo is null)
        {
            return Result<VeiculoResponse>.Falha(
                VeiculoErrorMessages.VeiculoNaoEncontrado,
                TipoErro.NaoEncontrado);
        }

        await _veiculoRepository.RemoverAsync(veiculo, cancellationToken);

        return Result<VeiculoResponse>.Ok(_mapper.Map<VeiculoResponse>(veiculo));
    }
}
