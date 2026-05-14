namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.AtualizarVeiculo;

public sealed record AtualizarVeiculoRequest(
    Guid VeiculoId,
    string Placa,
    string Marca,
    string Modelo,
    int Ano);