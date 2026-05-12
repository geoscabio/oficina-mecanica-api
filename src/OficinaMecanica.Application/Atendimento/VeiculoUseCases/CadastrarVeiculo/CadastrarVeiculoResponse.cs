namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.CadastrarVeiculo;

public sealed record CadastrarVeiculoResponse(
    Guid Id,
    Guid ClienteId,
    string Placa,
    string Marca,
    string Modelo,
    int Ano);
