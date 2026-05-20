using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Atendimento.VeiculoUseCases.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.AtualizarVeiculo;

public sealed class AtualizarVeiculoUseCase
{
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IValidator<AtualizarVeiculoRequest> _validator;
    private readonly IMapper _mapper;

    public AtualizarVeiculoUseCase(
        IVeiculoRepository veiculoRepository,
        IValidator<AtualizarVeiculoRequest> validator,
        IMapper mapper)
    {
        _veiculoRepository = veiculoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<VeiculoResponse>> ExecuteAsync(
        AtualizarVeiculoRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<VeiculoResponse>.Falha(
                validationResult.ObterMensagensErro(),
                TipoErro.Validacao);
        }

        var veiculo = await _veiculoRepository.ObterPorIdAsync(request.VeiculoId, cancellationToken);

        if (veiculo is null)
        {
            return Result<VeiculoResponse>.Falha(
                VeiculoErrorMessages.VeiculoNaoEncontrado,
                TipoErro.NaoEncontrado);
        }

        veiculo.Atualizar(
            Placa.Criar(request.Placa),
            request.Marca,
            request.Modelo,
            request.Ano);

        await _veiculoRepository.AtualizarAsync(veiculo, cancellationToken);

        return Result<VeiculoResponse>.Ok(_mapper.Map<VeiculoResponse>(veiculo));
    }
}
