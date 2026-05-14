using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.Responses;
using OficinaMecanica.Domain.GestaoEstoque.Entities;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;

namespace OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.ListarItensEstoque;

public sealed class ListarItensEstoqueUseCase
{
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly IValidator<ListarItensEstoqueRequest> _validator;
    private readonly IMapper _mapper;

    public ListarItensEstoqueUseCase(
        IEstoqueRepository estoqueRepository,
        IValidator<ListarItensEstoqueRequest> validator,
        IMapper mapper)
    {
        _estoqueRepository = estoqueRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<ItemEstoqueResponse>>> ExecuteAsync(
        ListarItensEstoqueRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<PagedResult<ItemEstoqueResponse>>.Falha(
                validationResult.Errors.First().ErrorMessage,
                TipoErro.Validacao);
        }

        var estoque = await _estoqueRepository.ObterAsync(cancellationToken);

        var itensEstoque = estoque?.ItensEstoque
            .OrderBy(item => item.PecaInsumoCatalogoId)
            .ToArray() ?? Array.Empty<ItemEstoque>();

        var itensPaginados = itensEstoque
            .Skip((request.Pagina - 1) * request.TamanhoPagina)
            .Take(request.TamanhoPagina)
            .ToArray();

        var response = _mapper.Map<IReadOnlyCollection<ItemEstoqueResponse>>(itensPaginados);

        var pagedResult = new PagedResult<ItemEstoqueResponse>(
            response,
            request.Pagina,
            request.TamanhoPagina,
            itensEstoque.Length);

        return Result<PagedResult<ItemEstoqueResponse>>.Ok(pagedResult);
    }
}