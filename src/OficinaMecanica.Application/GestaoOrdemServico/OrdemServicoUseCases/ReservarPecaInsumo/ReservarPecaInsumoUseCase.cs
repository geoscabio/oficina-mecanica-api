using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.GestaoEstoque.Messages;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;
using OficinaMecanica.Domain.Administrativo.Messages;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.Responses;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.GestaoEstoque.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;
using OficinaMecanica.Domain.GestaoEstoque.Aggregates;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ReservarPecaInsumo;

public sealed class ReservarPecaInsumoUseCase
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IPecaInsumoCatalogoRepository _pecaInsumoCatalogoRepository;
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<ReservarPecaInsumoRequest> _validator;
    private readonly IMapper _mapper;

    public ReservarPecaInsumoUseCase(
        IOrdemServicoRepository ordemServicoRepository,
        IPecaInsumoCatalogoRepository pecaInsumoCatalogoRepository,
        IEstoqueRepository estoqueRepository,
        IUnitOfWork unitOfWork,
        IValidator<ReservarPecaInsumoRequest> validator,
        IMapper mapper)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _pecaInsumoCatalogoRepository = pecaInsumoCatalogoRepository;
        _estoqueRepository = estoqueRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<OrdemServicoResponse>> ExecuteAsync(ReservarPecaInsumoRequest request, CancellationToken cancellationToken = default)
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

        var estoque = await _estoqueRepository.ObterAsync(cancellationToken);

        if (estoque is null)
        {
            return Result<OrdemServicoResponse>.Falha(EstoqueErrorMessages.EstoqueNaoEncontrado, TipoErro.NaoEncontrado);
        }

        var pecasInsumosCatalogo = await ObterPecasInsumosCatalogoAsync(request.PecasInsumos, cancellationToken);

        if (request.PecasInsumos.Any(pecaInsumo => !pecasInsumosCatalogo.ContainsKey(pecaInsumo.PecaInsumoCatalogoId)))
        {
            return Result<OrdemServicoResponse>.Falha(PecaInsumoCatalogoErrorMessages.PecaInsumoCatalogoNaoEncontrado, TipoErro.NaoEncontrado);
        }

        if (!ExisteEstoqueDisponivel(estoque, request.PecasInsumos))
        {
            return Result<OrdemServicoResponse>.Falha(EstoqueErrorMessages.EstoqueInsuficiente, TipoErro.RegraNegocio);
        }

        foreach (var pecaInsumo in request.PecasInsumos)
        {
            var pecaInsumoCatalogo = pecasInsumosCatalogo[pecaInsumo.PecaInsumoCatalogoId];

            ordemServico.ReservarPecaInsumo(pecaInsumoCatalogo.Id, pecaInsumo.Quantidade, pecaInsumoCatalogo.Valor);

            estoque.ReservarItens(pecaInsumoCatalogo.Id, pecaInsumo.Quantidade);
        }

        await _unitOfWork.ExecutarEmTransacaoAsync(
            async transactionCancellationToken =>
            {
                await _ordemServicoRepository.AtualizarAsync(ordemServico, transactionCancellationToken);
                await _estoqueRepository.AtualizarAsync(estoque, transactionCancellationToken);
            },
            cancellationToken);

        return Result<OrdemServicoResponse>.Ok(_mapper.Map<OrdemServicoResponse>(ordemServico));
    }

    private async Task<Dictionary<Guid, PecaInsumoCatalogo>> ObterPecasInsumosCatalogoAsync(IEnumerable<PecaInsumoRequest> pecasInsumos, CancellationToken cancellationToken)
    {
        var ids = pecasInsumos
            .Select(pecaInsumo => pecaInsumo.PecaInsumoCatalogoId)
            .Distinct()
            .ToArray();

        var pecasInsumosCatalogo = await _pecaInsumoCatalogoRepository.ObterPorIdsAsync(ids, cancellationToken);

        return pecasInsumosCatalogo.ToDictionary(pecaInsumoCatalogo => pecaInsumoCatalogo.Id);
    }

    private static bool ExisteEstoqueDisponivel(Estoque estoque, IEnumerable<PecaInsumoRequest> pecasInsumos)
    {
        foreach (var pecaInsumo in pecasInsumos)
        {
            if (!estoque.VerificarDisponibilidade(pecaInsumo.PecaInsumoCatalogoId, pecaInsumo.Quantidade))
            {
                return false;
            }
        }

        return true;
    }
}




