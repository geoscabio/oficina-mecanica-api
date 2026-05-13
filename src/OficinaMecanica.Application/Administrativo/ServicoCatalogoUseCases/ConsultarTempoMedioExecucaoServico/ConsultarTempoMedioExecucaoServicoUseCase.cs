using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Administrativo.Mappings;
using OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.Responses;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Administrativo.Messages;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;

namespace OficinaMecanica.Application.Administrativo.ServicoCatalogoUseCases.ConsultarTempoMedioExecucaoServico;

public sealed class ConsultarTempoMedioExecucaoServicoUseCase
{
    private readonly IServicoCatalogoRepository _servicoCatalogoRepository;
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IValidator<ConsultarTempoMedioExecucaoServicoRequest> _validator;
    private readonly IMapper _mapper;

    public ConsultarTempoMedioExecucaoServicoUseCase(
        IServicoCatalogoRepository servicoCatalogoRepository,
        IOrdemServicoRepository ordemServicoRepository,
        IValidator<ConsultarTempoMedioExecucaoServicoRequest> validator,
        IMapper mapper)
    {
        _servicoCatalogoRepository = servicoCatalogoRepository;
        _ordemServicoRepository = ordemServicoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<TempoMedioExecucaoServicoResponse>> ExecuteAsync(
        ConsultarTempoMedioExecucaoServicoRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<TempoMedioExecucaoServicoResponse>.Falha(
                validationResult.Errors.First().ErrorMessage,
                TipoErro.Validacao);
        }

        var servicoCatalogo = await _servicoCatalogoRepository.ObterPorIdAsync(
            request.ServicoCatalogoId,
            cancellationToken);

        if (servicoCatalogo is null)
        {
            return Result<TempoMedioExecucaoServicoResponse>.Falha(
                ServicoCatalogoErrorMessages.ServicoCatalogoNaoEncontrado,
                TipoErro.NaoEncontrado);
        }

        var tempoMedio = await _ordemServicoRepository.ObterTempoMedioExecucaoServicoAsync(
            servicoCatalogo.Id,
            cancellationToken);
        var response = _mapper.Map<TempoMedioExecucaoServicoResponse>(
            servicoCatalogo,
            opcao => opcao.Items[ServicoCatalogoMappingProfile.TempoMedioExecucaoEmMinutosKey] = tempoMedio);

        return Result<TempoMedioExecucaoServicoResponse>.Ok(response);
    }
}
