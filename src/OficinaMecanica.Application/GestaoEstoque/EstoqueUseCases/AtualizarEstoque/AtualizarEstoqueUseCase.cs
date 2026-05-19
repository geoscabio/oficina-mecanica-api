using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.Responses;
using OficinaMecanica.Application.GestaoEstoque.ValidationMessages;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;

namespace OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.AtualizarEstoque;

public sealed class AtualizarEstoqueUseCase
{
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly IValidator<AtualizarEstoqueRequest> _validator;
    private readonly IMapper _mapper;

    public AtualizarEstoqueUseCase(
        IEstoqueRepository estoqueRepository,
        IValidator<AtualizarEstoqueRequest> validator,
        IMapper mapper)
    {
        _estoqueRepository = estoqueRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<ItemEstoqueResponse>> ExecuteAsync(
        AtualizarEstoqueRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<ItemEstoqueResponse>.Falha(
                validationResult.Errors.First().ErrorMessage,
                TipoErro.Validacao);
        }

        var estoque = await _estoqueRepository.ObterAsync(cancellationToken);

        if (estoque is null
            || !estoque.ItensEstoque.Any(item => item.PecaInsumoCatalogoId == request.PecaInsumoCatalogoId))
        {
            return Result<ItemEstoqueResponse>.Falha(EstoqueValidationMessages.ItemEstoqueNaoEncontrado, TipoErro.NaoEncontrado);
        }

        var itemEstoque = estoque.AtualizarQuantidadeDisponivel(
            request.PecaInsumoCatalogoId,
            request.QuantidadeDisponivel);

        await _estoqueRepository.AtualizarAsync(estoque, cancellationToken);

        return Result<ItemEstoqueResponse>.Ok(_mapper.Map<ItemEstoqueResponse>(itemEstoque));
    }
}
