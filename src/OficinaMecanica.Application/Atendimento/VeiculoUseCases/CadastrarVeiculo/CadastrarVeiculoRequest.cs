namespace OficinaMecanica.Application.Atendimento.VeiculoUseCases.CadastrarVeiculo;

public sealed record CadastrarVeiculoRequest(
    Guid ClienteId,
    string Placa,
    string Marca,
    string Modelo,
    int Ano);
