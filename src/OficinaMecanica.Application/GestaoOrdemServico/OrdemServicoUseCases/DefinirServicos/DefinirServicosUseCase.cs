using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;
using OficinaMecanica.Domain.Administrativo.Messages;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.Responses;
using OficinaMecanica.Domain.Administrativo.Aggregates;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.DefinirServicos;

public sealed class DefinirServicosUseCase
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IServicoCatalogoRepository _servicoCatalogoRepository;
    private readonly IValidator<DefinirServicosRequest> _validator;
    private readonly IMapper _mapper;

    public DefinirServicosUseCase(
        IOrdemServicoRepository ordemServicoRepository,
        IServicoCatalogoRepository servicoCatalogoRepository,
        IValidator<DefinirServicosRequest> validator,
        IMapper mapper)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _servicoCatalogoRepository = servicoCatalogoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<OrdemServicoResponse>> ExecuteAsync(
        DefinirServicosRequest request,
        CancellationToken cancellationToken = default)
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

        var servicosCatalogo = await ObterServicosCatalogoAsync(request.ServicosCatalogoIds, cancellationToken);

        if (request.ServicosCatalogoIds.Any(servicoCatalogoId => !servicosCatalogo.ContainsKey(servicoCatalogoId)))
        {
            return Result<OrdemServicoResponse>.Falha(ServicoCatalogoErrorMessages.ServicoCatalogoNaoEncontrado, TipoErro.NaoEncontrado);
        }

        foreach (var servicoCatalogoId in request.ServicosCatalogoIds)
        {
            var servicoCatalogo = servicosCatalogo[servicoCatalogoId];

            ordemServico.DefinirServico(servicoCatalogo.Id, servicoCatalogo.Valor);
        }

        await _ordemServicoRepository.AtualizarAsync(ordemServico, cancellationToken);

        return Result<OrdemServicoResponse>.Ok(_mapper.Map<OrdemServicoResponse>(ordemServico));
    }

    private async Task<Dictionary<Guid, ServicoCatalogo>> ObterServicosCatalogoAsync(
        IEnumerable<Guid> servicosCatalogoIds,
        CancellationToken cancellationToken)
    {
        var ids = servicosCatalogoIds
            .Distinct()
            .ToArray();

        var servicosCatalogo = await _servicoCatalogoRepository.ObterPorIdsAsync(ids, cancellationToken);

        return servicosCatalogo.ToDictionary(servicoCatalogo => servicoCatalogo.Id);
    }
}




