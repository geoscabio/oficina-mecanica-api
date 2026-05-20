namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.Responses;

public sealed record OrdemServicoResponse(
    Guid Id,
    int Numero,
    string Status,
    decimal ValorTotal,
    DateTime? DataInicio,
    DateTime? DataFim,
    Guid VeiculoId,
    Guid MecanicoId,
    IReadOnlyCollection<ServicoOrdemServicoResponse> Servicos,
    IReadOnlyCollection<PecaInsumoOrdemServicoResponse> PecasInsumos);

public sealed record ServicoOrdemServicoResponse(Guid Id, Guid ServicoCatalogoId, string Status, decimal Valor, DateTime? DataInicio, DateTime? DataFim);

public sealed record PecaInsumoOrdemServicoResponse(Guid Id, Guid PecaInsumoCatalogoId, int Quantidade, decimal ValorUnitario, decimal ValorTotal);
