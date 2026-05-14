using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.Responses;
using OficinaMecanica.Domain.GestaoEstoque.Aggregates;
using OficinaMecanica.Domain.GestaoEstoque.Entities;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;

namespace OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.RegistrarEntradaEstoque;

public sealed class RegistrarEntradaEstoqueUseCase
{
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly IValidator<RegistrarEntradaEstoqueRequest> _validator;
    private readonly IMapper _mapper;

    public RegistrarEntradaEstoqueUseCase(
        IEstoqueRepository estoqueRepository,
        IValidator<RegistrarEntradaEstoqueRequest> validator,
        IMapper mapper)
    {
        _estoqueRepository = estoqueRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<ItemEstoqueResponse>> ExecuteAsync(
        RegistrarEntradaEstoqueRequest request,
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

        if (estoque is null)
        {
            var item = ItemEstoque.Criar(request.PecaInsumoCatalogoId, request.Quantidade);

            estoque = Estoque.Criar(new[] { item });

            await _estoqueRepository.AtualizarAsync(estoque, cancellationToken);

            return Result<ItemEstoqueResponse>.Ok(_mapper.Map<ItemEstoqueResponse>(item));
        }

        var itemEstoque = estoque.RegistrarEntrada(request.PecaInsumoCatalogoId, request.Quantidade);

        await _estoqueRepository.AtualizarAsync(estoque, cancellationToken);

        return Result<ItemEstoqueResponse>.Ok(_mapper.Map<ItemEstoqueResponse>(itemEstoque));
    }
}