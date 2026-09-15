using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.Responses;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarDiagnosticoOrdemServico;

public sealed class IniciarDiagnosticoOrdemServicoUseCase
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IValidator<IniciarDiagnosticoOrdemServicoRequest> _validator;
    private readonly IMapper _mapper;
    private readonly ILogger<IniciarDiagnosticoOrdemServicoUseCase> _logger;

    public IniciarDiagnosticoOrdemServicoUseCase(IOrdemServicoRepository ordemServicoRepository, IValidator<IniciarDiagnosticoOrdemServicoRequest> validator, IMapper mapper, ILogger<IniciarDiagnosticoOrdemServicoUseCase> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _validator = validator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<OrdemServicoResponse>> ExecuteAsync(IniciarDiagnosticoOrdemServicoRequest request, CancellationToken cancellationToken = default)
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

        ordemServico.IniciarDiagnostico();

        await _ordemServicoRepository.AtualizarAsync(ordemServico, cancellationToken);

        _logger.LogInformation(
            "Ordem de servico {OrdemServicoId} iniciou diagnostico com status {OrdemServicoStatus}. {operation} {bounded_context}",
            ordemServico.Id,
            ordemServico.Status,
            "IniciarDiagnosticoOrdemServico",
            "GestaoOrdemServico");

        return Result<OrdemServicoResponse>.Ok(_mapper.Map<OrdemServicoResponse>(ordemServico));
    }
}


