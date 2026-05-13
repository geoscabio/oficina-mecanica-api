namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.Responses;

public sealed record OrdemServicoResponse(
    Guid Id,
    int Numero,
    string Status,
    decimal ValorTotal,
    DateTime? DataInicio,
    DateTime? DataFim,
    Guid VeiculoId,
    Guid MecanicoId);
