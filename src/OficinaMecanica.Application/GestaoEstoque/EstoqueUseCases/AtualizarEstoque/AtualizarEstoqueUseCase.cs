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

        var itemEstoque = await _estoqueRepository.ObterItemPorPecaInsumoCatalogoIdAsync(
            request.PecaInsumoCatalogoId,
            cancellationToken);

        if (itemEstoque is null)
        {
            return Result<ItemEstoqueResponse>.Falha(EstoqueValidationMessages.ItemEstoqueNaoEncontrado, TipoErro.NaoEncontrado);
        }

        itemEstoque.AtualizarQuantidadeDisponivel(request.QuantidadeDisponivel);

        await _estoqueRepository.AtualizarItemAsync(itemEstoque, cancellationToken);

        return Result<ItemEstoqueResponse>.Ok(_mapper.Map<ItemEstoqueResponse>(itemEstoque));
    }
}