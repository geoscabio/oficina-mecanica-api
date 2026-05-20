using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Administrativo.MecanicoUseCases.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;

namespace OficinaMecanica.Application.Administrativo.MecanicoUseCases.ConsultarMecanico;

public sealed class ConsultarMecanicoUseCase
{
    private readonly IMecanicoRepository _mecanicoRepository;
    private readonly IValidator<ConsultarMecanicoRequest> _validator;
    private readonly IMapper _mapper;

    public ConsultarMecanicoUseCase(IMecanicoRepository mecanicoRepository, IValidator<ConsultarMecanicoRequest> validator, IMapper mapper)
    {
        _mecanicoRepository = mecanicoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<MecanicoResponse>> ExecuteAsync(ConsultarMecanicoRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<MecanicoResponse>.Falha(validationResult.ObterMensagensErro(), TipoErro.Validacao);
        }

        var mecanico = await _mecanicoRepository.ObterPorIdAsync(request.MecanicoId, cancellationToken);

        if (mecanico is null)
        {
            return Result<MecanicoResponse>.Falha(MecanicoErrorMessages.MecanicoNaoEncontrado, TipoErro.NaoEncontrado);
        }

        return Result<MecanicoResponse>.Ok(_mapper.Map<MecanicoResponse>(mecanico));
    }
}
