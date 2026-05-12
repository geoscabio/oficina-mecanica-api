using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculo;

public sealed class ConsultarVeiculoUseCase
{
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly ConsultarVeiculoValidator _validator;

    public ConsultarVeiculoUseCase(IVeiculoRepository veiculoRepository, ConsultarVeiculoValidator validator)
    {
        _veiculoRepository = veiculoRepository;
        _validator = validator;
    }

    public async Task<Result<ConsultarVeiculoResponse>> ExecuteAsync(
        ConsultarVeiculoRequest request,
        CancellationToken cancellationToken = default)
    {
        _validator.Validate(request);

        var veiculo = await _veiculoRepository.ObterPorIdAsync(request.Id, cancellationToken);

        if (veiculo is null)
        {
            return Result<ConsultarVeiculoResponse>.Falha("Veiculo nao encontrado.");
        }

        return Result<ConsultarVeiculoResponse>.Ok(MapearResponse(veiculo));
    }

    private static ConsultarVeiculoResponse MapearResponse(Veiculo veiculo)
    {
        return new ConsultarVeiculoResponse(
            veiculo.Id,
            veiculo.ClienteId,
            veiculo.Placa.NumeroPlaca,
            veiculo.Marca,
            veiculo.Modelo,
            veiculo.Ano);
    }
}
