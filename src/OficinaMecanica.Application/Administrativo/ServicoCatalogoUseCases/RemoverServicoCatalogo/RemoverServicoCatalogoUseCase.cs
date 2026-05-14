using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;

namespace OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.RemoverServicoCatalogo;

public sealed class RemoverServicoCatalogoUseCase
{
    private readonly IServicoCatalogoRepository _servicoCatalogoRepository;
    private readonly IValidator<RemoverServicoCatalogoRequest> _validator;
    private readonly IMapper _mapper;

    public RemoverServicoCatalogoUseCase(
        IServicoCatalogoRepository servicoCatalogoRepository,
        IValidator<RemoverServicoCatalogoRequest> validator,
        IMapper mapper)
    {
        _servicoCatalogoRepository = servicoCatalogoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<ServicoCatalogoResponse>> ExecuteAsync(
        RemoverServicoCatalogoRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<ServicoCatalogoResponse>.Falha(
                validationResult.Errors.First().ErrorMessage,
                TipoErro.Validacao);
        }

        var servicoCatalogo = await _servicoCatalogoRepository.ObterPorIdAsync(
            request.ServicoCatalogoId,
            cancellationToken);

        if (servicoCatalogo is null)
        {
            return Result<ServicoCatalogoResponse>.Falha(
                ServicoCatalogoErrorMessages.ServicoCatalogoNaoEncontrado,
                TipoErro.NaoEncontrado);
        }

        await _servicoCatalogoRepository.RemoverAsync(servicoCatalogo, cancellationToken);

        return Result<ServicoCatalogoResponse>.Ok(_mapper.Map<ServicoCatalogoResponse>(servicoCatalogo));
    }
}
