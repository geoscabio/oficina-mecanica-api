using AutoMapper;
using FluentValidation;
using OficinaMecanica.Application.Common;
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

    public async Task<Result<AbrirOrdemServicoResponse>> ExecuteAsync(
        AbrirOrdemServicoRequest request,
        CancellationToken cancellationToken = default)
    {
        _validator.ValidateAndThrow(request);

        var veiculo = await _veiculoRepository.ObterPorIdAsync(request.VeiculoId, cancellationToken);
        if (veiculo is null)
        {
            return Result<AbrirOrdemServicoResponse>.Falha("Veiculo nao encontrado.");
        }

        var mecanico = await _mecanicoRepository.ObterPorIdAsync(request.MecanicoId, cancellationToken);
        if (mecanico is null)
        {
            return Result<AbrirOrdemServicoResponse>.Falha("Mecanico nao encontrado.");
        }

        var ordemServico = OrdemServico.Abrir(request.VeiculoId, request.MecanicoId);

        await _ordemServicoRepository.AdicionarAsync(ordemServico, cancellationToken);

        return Result<AbrirOrdemServicoResponse>.Ok(_mapper.Map<AbrirOrdemServicoResponse>(ordemServico));
    }
}
