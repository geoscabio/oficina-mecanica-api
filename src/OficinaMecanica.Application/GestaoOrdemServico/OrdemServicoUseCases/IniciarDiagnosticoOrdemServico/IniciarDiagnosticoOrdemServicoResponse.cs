namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.IniciarDiagnosticoOrdemServico;

public sealed record IniciarDiagnosticoOrdemServicoResponse(
    Guid Id,
    int Numero,
    string Status,
    decimal ValorTotal,
    DateTime? DataInicio,
    DateTime? DataFim,
    Guid VeiculoId,
    Guid MecanicoId);
