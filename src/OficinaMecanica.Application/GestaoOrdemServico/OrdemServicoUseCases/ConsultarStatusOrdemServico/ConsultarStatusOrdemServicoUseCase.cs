using FluentValidation;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ConsultarStatusOrdemServico;

public sealed class ConsultarStatusOrdemServicoUseCase
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IValidator<ConsultarStatusOrdemServicoRequest> _validator;

    public ConsultarStatusOrdemServicoUseCase(
        IOrdemServicoRepository ordemServicoRepository,
        IValidator<ConsultarStatusOrdemServicoRequest> validator)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _validator = validator;
    }

    public async Task<Result<ConsultarStatusOrdemServicoResponse>> ExecuteAsync(
        ConsultarStatusOrdemServicoRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<ConsultarStatusOrdemServicoResponse>.Falha(
                validationResult.Errors.First().ErrorMessage,
                TipoErro.Validacao);
        }

        var ordemServico = await _ordemServicoRepository.ObterPorIdAsync(request.OrdemServicoId, cancellationToken);

        if (ordemServico is null)
        {
            return Result<ConsultarStatusOrdemServicoResponse>.Falha(
                OrdemServicoErrorMessages.OrdemServicoNaoEncontrada,
                TipoErro.NaoEncontrado);
        }

        var servicos = ordemServico.Servicos
            .Select(servico => new ServicoStatusResponse(
                servico.Id,
                servico.ServicoCatalogoId,
                servico.Status.ToString()))
            .ToArray();

        var response = new ConsultarStatusOrdemServicoResponse(
            ordemServico.Id,
            ordemServico.Numero,
            ordemServico.Status.ToString(),
            servicos);

        return Result<ConsultarStatusOrdemServicoResponse>.Ok(response);
    }
}
