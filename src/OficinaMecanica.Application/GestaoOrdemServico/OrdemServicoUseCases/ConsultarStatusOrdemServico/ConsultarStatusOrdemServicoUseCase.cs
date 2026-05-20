using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Messages;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ConsultarStatusOrdemServico;

public sealed class ConsultarStatusOrdemServicoUseCase
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IValidator<ConsultarStatusOrdemServicoRequest> _validator;
    private readonly IMapper _mapper;

    public ConsultarStatusOrdemServicoUseCase(IOrdemServicoRepository ordemServicoRepository, IValidator<ConsultarStatusOrdemServicoRequest> validator, IMapper mapper)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<ConsultarStatusOrdemServicoResponse>> ExecuteAsync(ConsultarStatusOrdemServicoRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<ConsultarStatusOrdemServicoResponse>.Falha(validationResult.ObterMensagensErro(), TipoErro.Validacao);
        }

        var ordemServico = await _ordemServicoRepository.ObterPorIdAsync(request.OrdemServicoId, cancellationToken);

        if (ordemServico is null)
        {
            return Result<ConsultarStatusOrdemServicoResponse>.Falha(OrdemServicoErrorMessages.OrdemServicoNaoEncontrada, TipoErro.NaoEncontrado);
        }

        return Result<ConsultarStatusOrdemServicoResponse>.Ok(_mapper.Map<ConsultarStatusOrdemServicoResponse>(ordemServico));
    }
}

