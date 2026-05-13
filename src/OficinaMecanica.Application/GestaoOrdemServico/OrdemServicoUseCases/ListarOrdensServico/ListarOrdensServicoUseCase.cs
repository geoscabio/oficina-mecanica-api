using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.Responses;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ListarOrdensServico;

public sealed class ListarOrdensServicoUseCase
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IValidator<ListarOrdensServicoRequest> _validator;
    private readonly IMapper _mapper;

    public ListarOrdensServicoUseCase(
        IOrdemServicoRepository ordemServicoRepository,
        IValidator<ListarOrdensServicoRequest> validator,
        IMapper mapper)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<OrdemServicoResponse>>> ExecuteAsync(
        ListarOrdensServicoRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<PagedResult<OrdemServicoResponse>>.Falha(
                validationResult.Errors.First().ErrorMessage,
                TipoErro.Validacao);
        }

        var ordensServico = await _ordemServicoRepository.ListarAsync(
            request.Pagina,
            request.TamanhoPagina,
            cancellationToken);
        var totalItens = await _ordemServicoRepository.ContarAsync(cancellationToken);
        var response = _mapper.Map<IReadOnlyCollection<OrdemServicoResponse>>(ordensServico);
        var pagedResult = new PagedResult<OrdemServicoResponse>(
            response,
            request.Pagina,
            request.TamanhoPagina,
            totalItens);

        return Result<PagedResult<OrdemServicoResponse>>.Ok(pagedResult);
    }
}
