namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.FinalizarServico;

public sealed record FinalizarServicoRequest(Guid OrdemServicoId, Guid ServicoId);
