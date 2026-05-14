using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.Responses;
using OficinaMecanica.Application.GestaoEstoque.ValidationMessages;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;

namespace OficinaMecanica.Application.GestaoEstoque.EstoqueUseCases.ConsultarItemEstoque;

public sealed class ConsultarItemEstoqueUseCase
{
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly IValidator<ConsultarItemEstoqueRequest> _validator;
    private readonly IMapper _mapper;

    public ConsultarItemEstoqueUseCase(
        IEstoqueRepository estoqueRepository,
        IValidator<ConsultarItemEstoqueRequest> validator,
        IMapper mapper)
    {
        _estoqueRepository = estoqueRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<ItemEstoqueResponse>> ExecuteAsync(
        ConsultarItemEstoqueRequest request,
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

        var itemEstoque = estoque?.ItensEstoque
                                  .FirstOrDefault(item => item.Id == request.ItemEstoqueId);

        if (itemEstoque is null)
        {
            return Result<ItemEstoqueResponse>.Falha(EstoqueValidationMessages.ItemEstoqueNaoEncontrado, TipoErro.NaoEncontrado);
        }

        var response = _mapper.Map<ItemEstoqueResponse>(itemEstoque);

        return Result<ItemEstoqueResponse>.Ok(response);
    }
}