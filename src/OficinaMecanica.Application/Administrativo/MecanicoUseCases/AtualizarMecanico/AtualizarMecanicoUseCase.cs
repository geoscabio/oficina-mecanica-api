using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;

namespace OficinaMecanica.Application.Administrativo.MecanicoUseCases.AtualizarMecanico;

public sealed class AtualizarMecanicoUseCase
{
    private readonly IMecanicoRepository _mecanicoRepository;
    private readonly IValidator<AtualizarMecanicoRequest> _validator;
    private readonly IMapper _mapper;

    public AtualizarMecanicoUseCase(
        IMecanicoRepository mecanicoRepository,
        IValidator<AtualizarMecanicoRequest> validator,
        IMapper mapper)
    {
        _mecanicoRepository = mecanicoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<MecanicoResponse>> ExecuteAsync(
        AtualizarMecanicoRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<MecanicoResponse>.Falha(
                validationResult.Errors.First().ErrorMessage,
                TipoErro.Validacao);
        }

        var mecanico = await _mecanicoRepository.ObterPorIdAsync(request.MecanicoId, cancellationToken);

        if (mecanico is null)
        {
            return Result<MecanicoResponse>.Falha(
                MecanicoErrorMessages.MecanicoNaoEncontrado,
                TipoErro.NaoEncontrado);
        }

        mecanico.Atualizar(request.Nome, request.Funcional);

        await _mecanicoRepository.AtualizarAsync(mecanico, cancellationToken);

        return Result<MecanicoResponse>.Ok(_mapper.Map<MecanicoResponse>(mecanico));
    }
}