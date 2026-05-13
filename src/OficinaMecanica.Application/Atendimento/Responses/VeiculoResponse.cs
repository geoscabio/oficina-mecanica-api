namespace OficinaMecanica.Application.Atendimento.Responses;

public sealed record VeiculoResponse(
    Guid Id,
    Guid ClienteId,
    string Placa,
    string Marca,
    string Modelo,
    int Ano);
