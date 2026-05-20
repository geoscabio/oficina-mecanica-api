namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.Responses;

public sealed record TempoMedioExecucaoServicoResponse(Guid ServicoCatalogoId, string Descricao, decimal Valor, double? TempoMedioExecucaoEmMinutos);
