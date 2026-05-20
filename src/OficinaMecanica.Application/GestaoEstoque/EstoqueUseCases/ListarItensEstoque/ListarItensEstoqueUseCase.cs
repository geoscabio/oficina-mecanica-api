using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.Responses;
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
                validationResult.ObterMensagensErro(),
                TipoErro.Validacao);
        }

        var itensEstoque = await _estoqueRepository.ListarItensAsync(request.Pagina, request.TamanhoPagina, cancellationToken);

        var totalItens = await _estoqueRepository.ContarItensAsync(cancellationToken);

        var response = _mapper.Map<IReadOnlyCollection<ItemEstoqueResponse>>(itensEstoque);

        var pagedResult = new PagedResult<ItemEstoqueResponse>(
            response,
            request.Pagina,
            request.TamanhoPagina,
            totalItens);

        return Result<PagedResult<ItemEstoqueResponse>>.Ok(pagedResult);
    }
}
