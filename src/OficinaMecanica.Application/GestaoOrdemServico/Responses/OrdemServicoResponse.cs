namespace OficinaMecanica.Application.GestaoOrdemServico.Responses;

public sealed record OrdemServicoResponse(
    Guid Id,
    int Numero,
    string Status,
    decimal ValorTotal,
    DateTime? DataInicio,
    DateTime? DataFim,
    Guid VeiculoId,
    Guid MecanicoId);
