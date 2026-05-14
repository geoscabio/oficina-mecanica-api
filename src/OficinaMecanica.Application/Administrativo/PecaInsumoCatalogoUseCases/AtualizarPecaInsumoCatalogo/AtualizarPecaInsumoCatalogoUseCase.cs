using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;

namespace OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.AtualizarPecaInsumoCatalogo;

public sealed class AtualizarPecaInsumoCatalogoUseCase
{
    private readonly IPecaInsumoCatalogoRepository _pecaInsumoCatalogoRepository;
    private readonly IValidator<AtualizarPecaInsumoCatalogoRequest> _validator;
    private readonly IMapper _mapper;

    public AtualizarPecaInsumoCatalogoUseCase(
        IPecaInsumoCatalogoRepository pecaInsumoCatalogoRepository,
        IValidator<AtualizarPecaInsumoCatalogoRequest> validator,
        IMapper mapper)
    {
        _pecaInsumoCatalogoRepository = pecaInsumoCatalogoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<PecaInsumoCatalogoResponse>> ExecuteAsync(
        AtualizarPecaInsumoCatalogoRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<PecaInsumoCatalogoResponse>.Falha(
                validationResult.Errors.First().ErrorMessage,
                TipoErro.Validacao);
        }

        var pecaInsumoCatalogo = await _pecaInsumoCatalogoRepository.ObterPorIdAsync(
            request.PecaInsumoCatalogoId,
            cancellationToken);

        if (pecaInsumoCatalogo is null)
        {
            return Result<PecaInsumoCatalogoResponse>.Falha(PecaInsumoCatalogoErrorMessages.PecaInsumoCatalogoNaoEncontrado, TipoErro.NaoEncontrado);
        }

        pecaInsumoCatalogo.Atualizar(
            request.Descricao,
            request.Tipo,
            request.Valor);

        await _pecaInsumoCatalogoRepository.AtualizarAsync(pecaInsumoCatalogo, cancellationToken);

        return Result<PecaInsumoCatalogoResponse>.Ok(_mapper.Map<PecaInsumoCatalogoResponse>(pecaInsumoCatalogo));
    }
}