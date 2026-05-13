using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Application.GestaoOrdemServico.Responses;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarDiagnosticoOrdemServico;

public sealed class IniciarDiagnosticoOrdemServicoUseCase
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IValidator<IniciarDiagnosticoOrdemServicoRequest> _validator;
    private readonly IMapper _mapper;

    public IniciarDiagnosticoOrdemServicoUseCase(
        IOrdemServicoRepository ordemServicoRepository,
        IValidator<IniciarDiagnosticoOrdemServicoRequest> validator,
        IMapper mapper)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<OrdemServicoResponse>> ExecuteAsync(
        IniciarDiagnosticoOrdemServicoRequest request,
        CancellationToken cancellationToken = default)
    {
        _validator.ValidateAndThrow(request);

        var ordemServico = await _ordemServicoRepository.ObterPorIdAsync(request.OrdemServicoId, cancellationToken);

        if (ordemServico is null)
        {
            return Result<OrdemServicoResponse>.Falha("Ordem de servico nao encontrada.");
        }

        ordemServico.IniciarDiagnostico();

        await _ordemServicoRepository.AtualizarAsync(ordemServico, cancellationToken);

        return Result<OrdemServicoResponse>.Ok(_mapper.Map<OrdemServicoResponse>(ordemServico));
    }
}
