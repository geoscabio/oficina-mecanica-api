using OficinaMecanica.Application.Common;
using OficinaMecanica.Domain.Atendimento.Aggregates;
using OficinaMecanica.Domain.Atendimento.Interfaces;
using OficinaMecanica.Domain.Atendimento.ValueObjects;

namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.CadastrarVeiculo;

public sealed class CadastrarVeiculoUseCase
{
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IClienteRepository _clienteRepository;
    private readonly CadastrarVeiculoValidator _validator;

    public CadastrarVeiculoUseCase(
        IVeiculoRepository veiculoRepository,
        IClienteRepository clienteRepository,
        CadastrarVeiculoValidator validator)
    {
        _veiculoRepository = veiculoRepository;
        _clienteRepository = clienteRepository;
        _validator = validator;
    }

    public async Task<Result<CadastrarVeiculoResponse>> ExecuteAsync(
        CadastrarVeiculoRequest request,
        CancellationToken cancellationToken = default)
    {
        _validator.Validate(request);

        var cliente = await _clienteRepository.ObterPorIdAsync(request.ClienteId, cancellationToken);

        if (cliente is null)
        {
            return Result<CadastrarVeiculoResponse>.Falha("Cliente nao encontrado.");
        }

        var placa = Placa.Criar(request.Placa);
        var veiculoExistente = await _veiculoRepository.ObterPorPlacaAsync(placa.NumeroPlaca, cancellationToken);

        if (veiculoExistente is not null)
        {
            return Result<CadastrarVeiculoResponse>.Falha("Veiculo ja cadastrado para a placa informada.");
        }

        var veiculo = Veiculo.Criar(request.ClienteId, placa, request.Marca, request.Modelo, request.Ano);

        await _veiculoRepository.AdicionarAsync(veiculo, cancellationToken);

        return Result<CadastrarVeiculoResponse>.Ok(new CadastrarVeiculoResponse(
            veiculo.Id,
            veiculo.ClienteId,
            veiculo.Placa.NumeroPlaca,
            veiculo.Marca,
            veiculo.Modelo,
            veiculo.Ano));
    }
}
