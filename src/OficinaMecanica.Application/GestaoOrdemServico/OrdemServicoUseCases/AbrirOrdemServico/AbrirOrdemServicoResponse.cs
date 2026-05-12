namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.AbrirOrdemServico;

public sealed record AbrirOrdemServicoResponse(
    Guid Id,
    int Numero,
    string Status,
    decimal ValorTotal,
    DateTime? DataInicio,
    DateTime? DataFim,
    Guid VeiculoId,
    Guid MecanicoId);
