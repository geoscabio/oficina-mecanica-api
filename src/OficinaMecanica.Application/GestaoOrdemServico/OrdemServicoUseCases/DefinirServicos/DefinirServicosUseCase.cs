using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.Responses;
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
        _validator.ValidateAndThrow(request);

        var ordemServico = await _ordemServicoRepository.ObterPorIdAsync(request.OrdemServicoId, cancellationToken);

        if (ordemServico is null)
        {
            return Result<OrdemServicoResponse>.Falha("Ordem de servico nao encontrada.");
        }

        var servicosCatalogo = await ObterServicosCatalogoAsync(request.ServicosCatalogoIds, cancellationToken);

        if (servicosCatalogo.Count != request.ServicosCatalogoIds.Count)
        {
            return Result<OrdemServicoResponse>.Falha("Servico do catalogo nao encontrado.");
        }

        foreach (var servicoCatalogo in servicosCatalogo)
        {
            ordemServico.DefinirServico(servicoCatalogo.Id, servicoCatalogo.Valor);
        }

        await _ordemServicoRepository.AtualizarAsync(ordemServico, cancellationToken);

        return Result<OrdemServicoResponse>.Ok(_mapper.Map<OrdemServicoResponse>(ordemServico));
    }

    private async Task<List<ServicoCatalogo>> ObterServicosCatalogoAsync(
        IEnumerable<Guid> servicosCatalogoIds,
        CancellationToken cancellationToken)
    {
        var servicosCatalogo = new List<ServicoCatalogo>();

        foreach (var servicoCatalogoId in servicosCatalogoIds)
        {
            var servicoCatalogo = await _servicoCatalogoRepository.ObterPorIdAsync(
                servicoCatalogoId,
                cancellationToken);

            if (servicoCatalogo is not null)
            {
                servicosCatalogo.Add(servicoCatalogo);
            }
        }

        return servicosCatalogo;
    }
}
