using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;

namespace OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.ConsultarPecaInsumoCatalogo;

public sealed class ConsultarPecaInsumoCatalogoUseCase
{
    private readonly IPecaInsumoCatalogoRepository _pecaInsumoCatalogoRepository;
    private readonly IValidator<ConsultarPecaInsumoCatalogoRequest> _validator;
    private readonly IMapper _mapper;

    public ConsultarPecaInsumoCatalogoUseCase(
        IPecaInsumoCatalogoRepository pecaInsumoCatalogoRepository,
        IValidator<ConsultarPecaInsumoCatalogoRequest> validator,
        IMapper mapper)
    {
        _pecaInsumoCatalogoRepository = pecaInsumoCatalogoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<PecaInsumoCatalogoResponse>> ExecuteAsync(
        ConsultarPecaInsumoCatalogoRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<PecaInsumoCatalogoResponse>.Falha(
                validationResult.Errors.First().ErrorMessage,
                TipoErro.Validacao);
        }

        var item = await _pecaInsumoCatalogoRepository.ObterPorIdAsync(
            request.PecaInsumoCatalogoId,
            cancellationToken);

        if (item is null)
        {
            return Result<PecaInsumoCatalogoResponse>.Falha(
                PecaInsumoCatalogoErrorMessages.PecaInsumoCatalogoNaoEncontrado,
                TipoErro.NaoEncontrado);
        }

        return Result<PecaInsumoCatalogoResponse>.Ok(_mapper.Map<PecaInsumoCatalogoResponse>(item));
    }
}