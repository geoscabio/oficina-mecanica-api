namespace OficinaMecanica.API.Atendimento.Requests;

public sealed record AtualizarVeiculoApiRequest(
    string Placa,
    string Marca,
    string Modelo,
    int Ano);
