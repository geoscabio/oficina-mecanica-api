using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Administrativo.Interfaces;

namespace OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ListarServicosCatalogo;

public sealed class ListarServicosCatalogoUseCase
{
    private readonly IServicoCatalogoRepository _servicoCatalogoRepository;
    private readonly IValidator<ListarServicosCatalogoRequest> _validator;
    private readonly IMapper _mapper;

    public ListarServicosCatalogoUseCase(
        IServicoCatalogoRepository servicoCatalogoRepository,
        IValidator<ListarServicosCatalogoRequest> validator,
        IMapper mapper)
    {
        _servicoCatalogoRepository = servicoCatalogoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<ServicoCatalogoResponse>>> ExecuteAsync(
        ListarServicosCatalogoRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<PagedResult<ServicoCatalogoResponse>>.Falha(
                validationResult.Errors.First().ErrorMessage,
                TipoErro.Validacao);
        }

        var servicosCatalogo = await _servicoCatalogoRepository.ListarAsync(
            request.Pagina,
            request.TamanhoPagina,
            cancellationToken);
        var totalItens = await _servicoCatalogoRepository.ContarAsync(cancellationToken);
        var response = _mapper.Map<IReadOnlyCollection<ServicoCatalogoResponse>>(servicosCatalogo);
        var pagedResult = new PagedResult<ServicoCatalogoResponse>(
            response,
            request.Pagina,
            request.TamanhoPagina,
            totalItens);

        return Result<PagedResult<ServicoCatalogoResponse>>.Ok(pagedResult);
    }
}
