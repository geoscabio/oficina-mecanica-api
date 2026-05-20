using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;

namespace OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.RemoverPecaInsumoCatalogo;

public sealed class RemoverPecaInsumoCatalogoUseCase
{
    private readonly IPecaInsumoCatalogoRepository _pecaInsumoCatalogoRepository;
    private readonly IValidator<RemoverPecaInsumoCatalogoRequest> _validator;
    private readonly IMapper _mapper;

    public RemoverPecaInsumoCatalogoUseCase(
        IPecaInsumoCatalogoRepository pecaInsumoCatalogoRepository,
        IValidator<RemoverPecaInsumoCatalogoRequest> validator,
        IMapper mapper)
    {
        _pecaInsumoCatalogoRepository = pecaInsumoCatalogoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<PecaInsumoCatalogoResponse>> ExecuteAsync(
        RemoverPecaInsumoCatalogoRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<PecaInsumoCatalogoResponse>.Falha(
                validationResult.ObterMensagensErro(),
                TipoErro.Validacao);
        }

        var pecaInsumoCatalogo = await _pecaInsumoCatalogoRepository.ObterPorIdAsync(
            request.PecaInsumoCatalogoId,
            cancellationToken);

        if (pecaInsumoCatalogo is null)
        {
            return Result<PecaInsumoCatalogoResponse>.Falha(
                PecaInsumoCatalogoErrorMessages.PecaInsumoCatalogoNaoEncontrado,
                TipoErro.NaoEncontrado);
        }

        await _pecaInsumoCatalogoRepository.RemoverAsync(
            pecaInsumoCatalogo,
            cancellationToken);

        return Result<PecaInsumoCatalogoResponse>.Ok(
            _mapper.Map<PecaInsumoCatalogoResponse>(pecaInsumoCatalogo));
    }
}
