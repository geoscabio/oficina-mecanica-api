using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.Responses;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;
using OficinaMecanica.Domain.GestaoEstoque.Messages;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.CancelarOrdemServico;

public sealed class CancelarOrdemServicoUseCase
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly IValidator<CancelarOrdemServicoRequest> _validator;
    private readonly IMapper _mapper;

    public CancelarOrdemServicoUseCase(
        IOrdemServicoRepository ordemServicoRepository,
        IEstoqueRepository estoqueRepository,
        IValidator<CancelarOrdemServicoRequest> validator,
        IMapper mapper)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _estoqueRepository = estoqueRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<OrdemServicoResponse>> ExecuteAsync(
        CancelarOrdemServicoRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<OrdemServicoResponse>.Falha(validationResult.Errors.First().ErrorMessage, TipoErro.Validacao);
        }

        var ordemServico = await _ordemServicoRepository.ObterPorIdAsync(request.OrdemServicoId, cancellationToken);

        if (ordemServico is null)
        {
            return Result<OrdemServicoResponse>.Falha(OrdemServicoErrorMessages.OrdemServicoNaoEncontrada, TipoErro.NaoEncontrado);
        }

        var deveEstornarEstoque = ordemServico.PecasInsumos.Count > 0;
        var estoque = deveEstornarEstoque
            ? await _estoqueRepository.ObterAsync(cancellationToken)
            : null;

        if (deveEstornarEstoque && estoque is null)
        {
            return Result<OrdemServicoResponse>.Falha(EstoqueErrorMessages.EstoqueNaoEncontrado, TipoErro.NaoEncontrado);
        }

        ordemServico.Cancelar(request.Motivo);

        if (deveEstornarEstoque)
        {
            foreach (var pecaInsumo in ordemServico.PecasInsumos)
            {
                estoque!.EstornarItens(pecaInsumo.PecaInsumoCatalogoId, pecaInsumo.Quantidade);
            }
        }

        await _ordemServicoRepository.AtualizarAsync(ordemServico, cancellationToken);
        if (estoque is not null)
        {
            await _estoqueRepository.AtualizarAsync(estoque, cancellationToken);
        }

        return Result<OrdemServicoResponse>.Ok(_mapper.Map<OrdemServicoResponse>(ordemServico));
    }

}
