namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.ConsultarVeiculo;

public sealed record ConsultarVeiculoResponse(
    Guid Id,
    Guid ClienteId,
    string Placa,
    string Marca,
    string Modelo,
    int Ano);
