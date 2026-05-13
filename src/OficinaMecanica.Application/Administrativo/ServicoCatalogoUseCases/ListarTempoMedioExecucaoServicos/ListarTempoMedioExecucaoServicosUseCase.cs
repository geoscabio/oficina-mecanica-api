using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Administrativo.Mappings;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;
using OficinaMecanica.Domain.Administrativo.Interfaces;

namespace OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ListarTempoMedioExecucaoServicos;

public sealed class ListarTempoMedioExecucaoServicosUseCase
{
    private readonly IServicoCatalogoRepository _servicoCatalogoRepository;
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IValidator<ListarTempoMedioExecucaoServicosRequest> _validator;
    private readonly IMapper _mapper;

    public ListarTempoMedioExecucaoServicosUseCase(
        IServicoCatalogoRepository servicoCatalogoRepository,
        IOrdemServicoRepository ordemServicoRepository,
        IValidator<ListarTempoMedioExecucaoServicosRequest> validator,
        IMapper mapper)
    {
        _servicoCatalogoRepository = servicoCatalogoRepository;
        _ordemServicoRepository = ordemServicoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<TempoMedioExecucaoServicoResponse>>> ExecuteAsync(
        ListarTempoMedioExecucaoServicosRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<PagedResult<TempoMedioExecucaoServicoResponse>>.Falha(
                validationResult.Errors.First().ErrorMessage,
                TipoErro.Validacao);
        }

        var servicosCatalogo = await _servicoCatalogoRepository.ListarAsync(
            request.Pagina,
            request.TamanhoPagina,
            cancellationToken);
        var totalItens = await _servicoCatalogoRepository.ContarAsync(cancellationToken);
        var servicosCatalogoIds = servicosCatalogo
            .Select(servicoCatalogo => servicoCatalogo.Id)
            .ToArray();
        var temposMedios = servicosCatalogoIds.Length == 0
            ? new Dictionary<Guid, double>()
            : await _ordemServicoRepository.ListarTemposMediosExecucaoServicosAsync(
                servicosCatalogoIds,
                cancellationToken);

        var response = servicosCatalogo
            .Select(servicoCatalogo =>
            {
                var tempoMedio = temposMedios.TryGetValue(servicoCatalogo.Id, out var valor)
                    ? valor
                    : (double?)null;

                return _mapper.Map<TempoMedioExecucaoServicoResponse>(
                    servicoCatalogo,
                    opcao => opcao.Items[ServicoCatalogoMappingProfile.TempoMedioExecucaoEmMinutosKey] = tempoMedio);
            })
            .ToArray();
        var pagedResult = new PagedResult<TempoMedioExecucaoServicoResponse>(
            response,
            request.Pagina,
            request.TamanhoPagina,
            totalItens);

        return Result<PagedResult<TempoMedioExecucaoServicoResponse>>.Ok(pagedResult);
    }
}
