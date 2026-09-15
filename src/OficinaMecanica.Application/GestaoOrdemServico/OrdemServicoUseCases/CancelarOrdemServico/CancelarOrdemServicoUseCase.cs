using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CancelarOrdemServicoRequest> _validator;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelarOrdemServicoUseCase> _logger;

    public CancelarOrdemServicoUseCase(IOrdemServicoRepository ordemServicoRepository, IEstoqueRepository estoqueRepository, IUnitOfWork unitOfWork, IValidator<CancelarOrdemServicoRequest> validator, IMapper mapper, ILogger<CancelarOrdemServicoUseCase> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _estoqueRepository = estoqueRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<OrdemServicoResponse>> ExecuteAsync(CancelarOrdemServicoRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<OrdemServicoResponse>.Falha(validationResult.ObterMensagensErro(), TipoErro.Validacao);
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
            _logger.LogWarning(
                "Falha ao cancelar a ordem de servico {OrdemServicoId}: {failure_reason}. {operation} {bounded_context}",
                ordemServico.Id,
                EstoqueErrorMessages.EstoqueNaoEncontrado,
                "CancelarOrdemServico",
                "GestaoOrdemServico");

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

        if (estoque is null)
        {
            await _ordemServicoRepository.AtualizarAsync(ordemServico, cancellationToken);
        }
        else
        {
            await _unitOfWork.ExecutarEmTransacaoAsync(
                async transactionCancellationToken =>
                {
                    await _ordemServicoRepository.AtualizarAsync(ordemServico, transactionCancellationToken);
                    await _estoqueRepository.AtualizarAsync(estoque, transactionCancellationToken);
                },
                cancellationToken);
        }

        _logger.LogInformation(
            "Ordem de servico {OrdemServicoId} cancelada com status {OrdemServicoStatus}. {operation} {bounded_context}",
            ordemServico.Id,
            ordemServico.Status,
            "CancelarOrdemServico",
            "GestaoOrdemServico");

        return Result<OrdemServicoResponse>.Ok(_mapper.Map<OrdemServicoResponse>(ordemServico));
    }

}
