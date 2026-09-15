using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.Responses;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarExecucaoOrdemServico;

public sealed class IniciarExecucaoOrdemServicoUseCase
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IValidator<IniciarExecucaoOrdemServicoRequest> _validator;
    private readonly IMapper _mapper;
    private readonly ILogger<IniciarExecucaoOrdemServicoUseCase> _logger;

    public IniciarExecucaoOrdemServicoUseCase(IOrdemServicoRepository ordemServicoRepository, IValidator<IniciarExecucaoOrdemServicoRequest> validator, IMapper mapper, ILogger<IniciarExecucaoOrdemServicoUseCase> logger)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _validator = validator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<Result<OrdemServicoResponse>> ExecuteAsync(IniciarExecucaoOrdemServicoRequest request, CancellationToken cancellationToken = default)
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

        ordemServico.IniciarExecucao();

        await _ordemServicoRepository.AtualizarAsync(ordemServico, cancellationToken);

        _logger.LogInformation(
            "Ordem de servico {OrdemServicoId} iniciou execucao com status {OrdemServicoStatus}. {operation} {bounded_context}",
            ordemServico.Id,
            ordemServico.Status,
            "IniciarExecucaoOrdemServico",
            "GestaoOrdemServico");

        return Result<OrdemServicoResponse>.Ok(_mapper.Map<OrdemServicoResponse>(ordemServico));
    }
}
