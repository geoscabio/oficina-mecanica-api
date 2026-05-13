using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Administrativo.Messages;
using OficinaMecanica.Domain.Atendimento.Messages;
using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.Responses;
using OficinaMecanica.Domain.Administrativo.Interfaces;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.GestaoOrdemServico.Aggregates;
using OficinaMecanica.Domain.GestaoOrdemServico.Interfaces;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.AbrirOrdemServico;

public sealed class AbrirOrdemServicoUseCase
{
    private readonly IOrdemServicoRepository _ordemServicoRepository;
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IMecanicoRepository _mecanicoRepository;
    private readonly IValidator<AbrirOrdemServicoRequest> _validator;
    private readonly IMapper _mapper;

    public AbrirOrdemServicoUseCase(
        IOrdemServicoRepository ordemServicoRepository,
        IVeiculoRepository veiculoRepository,
        IMecanicoRepository mecanicoRepository,
        IValidator<AbrirOrdemServicoRequest> validator,
        IMapper mapper)
    {
        _ordemServicoRepository = ordemServicoRepository;
        _veiculoRepository = veiculoRepository;
        _mecanicoRepository = mecanicoRepository;
        _validator = validator;
        _mapper = mapper;
    }

    public async Task<Result<OrdemServicoResponse>> ExecuteAsync(
        AbrirOrdemServicoRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Result<OrdemServicoResponse>.Falha(validationResult.Errors.First().ErrorMessage, TipoErro.Validacao);
        }

        var veiculo = await _veiculoRepository.ObterPorIdAsync(request.VeiculoId, cancellationToken);
        if (veiculo is null)
        {
            return Result<OrdemServicoResponse>.Falha(VeiculoErrorMessages.VeiculoNaoEncontrado, TipoErro.NaoEncontrado);
        }

        var mecanico = await _mecanicoRepository.ObterPorIdAsync(request.MecanicoId, cancellationToken);
        if (mecanico is null)
        {
            return Result<OrdemServicoResponse>.Falha(MecanicoErrorMessages.MecanicoNaoEncontrado, TipoErro.NaoEncontrado);
        }

        var ordemServico = OrdemServico.Abrir(request.VeiculoId, request.MecanicoId);

        await _ordemServicoRepository.AdicionarAsync(ordemServico, cancellationToken);

        return Result<OrdemServicoResponse>.Ok(_mapper.Map<OrdemServicoResponse>(ordemServico));
    }
}



