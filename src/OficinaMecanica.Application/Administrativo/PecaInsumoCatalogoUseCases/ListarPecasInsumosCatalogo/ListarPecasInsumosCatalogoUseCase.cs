using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Administrativo.Interfaces;

namespace OficinaMecanica.Application.Administrativo.PecaInsumoCatalogoUseCases.ListarPecasInsumosCatalogo;

public sealed class ListarPecasInsumosCatalogoUseCase
{
    private readonly IPecaInsumoCatalogoRepository _pecaInsumoCatalogoRepository;
    private readonly IValidator<ListarPecasInsumosCatalogoRequest> _validator;
    private readonly IMapper _mapper;

    public ListarPecasInsumosCatalogoUseCase(
        IPecaInsumoCatalogoRepository pecaInsumoCatalogoRepository,
        IValidator<ListarPecasInsumosCatalogoRequest> validator,
        IMapper mapper)
    {
        _pecaInsumoCatalogoRepository = pecaInsumoCatalogoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<PecaInsumoCatalogoResponse>>> ExecuteAsync(
        ListarPecasInsumosCatalogoRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<PagedResult<PecaInsumoCatalogoResponse>>.Falha(
                validationResult.ObterMensagensErro(),
                TipoErro.Validacao);
        }

        var itens = await _pecaInsumoCatalogoRepository.ListarAsync(
            request.Pagina,
            request.TamanhoPagina,
            cancellationToken);

        var totalItens = await _pecaInsumoCatalogoRepository.ContarAsync(cancellationToken);

        var response = _mapper.Map<IReadOnlyCollection<PecaInsumoCatalogoResponse>>(itens);

        var pagedResult = new PagedResult<PecaInsumoCatalogoResponse>(
            response,
            request.Pagina,
            request.TamanhoPagina,
            totalItens);

        return Result<PagedResult<PecaInsumoCatalogoResponse>>.Ok(pagedResult);
    }
}
