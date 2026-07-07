using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.Responses;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;
using OficinaMecanica.Domain.GestaoEstoque.Messages;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.NotificarDecisaoOrcamento;

public sealed class NotificarDecisaoOrcamentoUseCase
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<NotificarDecisaoOrcamentoRequest> _validator;
    private readonly IMapper _mapper;

    public NotificarDecisaoOrcamentoUseCase(IOrdemServicoRepository ordemServicoRepository, IEstoqueRepository estoqueRepository, IUnitOfWork unitOfWork, IValidator<NotificarDecisaoOrcamentoRequest> validator, IMapper mapper)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _estoqueRepository = estoqueRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<OrdemServicoResponse>> ExecuteAsync(NotificarDecisaoOrcamentoRequest request, CancellationToken cancellationToken = default)
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

        return request.Decisao == DecisaoOrcamento.Aprovado
            ? await AprovarOrcamentoAsync(ordemServico, cancellationToken)
            : await RecusarOrcamentoAsync(ordemServico, cancellationToken);
    }

    private async Task<Result<OrdemServicoResponse>> AprovarOrcamentoAsync(OrdemServico ordemServico, CancellationToken cancellationToken)
    {
        ordemServico.IniciarExecucao();

        await _ordemServicoRepository.AtualizarAsync(ordemServico, cancellationToken);

        return Result<OrdemServicoResponse>.Ok(_mapper.Map<OrdemServicoResponse>(ordemServico));
    }

    private async Task<Result<OrdemServicoResponse>> RecusarOrcamentoAsync(OrdemServico ordemServico, CancellationToken cancellationToken)
    {
        var deveEstornarEstoque = ordemServico.PecasInsumos.Count > 0;
        var estoque = deveEstornarEstoque
            ? await _estoqueRepository.ObterAsync(cancellationToken)
            : null;

        if (deveEstornarEstoque && estoque is null)
        {
            return Result<OrdemServicoResponse>.Falha(EstoqueErrorMessages.EstoqueNaoEncontrado, TipoErro.NaoEncontrado);
        }

        ordemServico.RecusarOrcamento();

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

        return Result<OrdemServicoResponse>.Ok(_mapper.Map<OrdemServicoResponse>(ordemServico));
    }
}
