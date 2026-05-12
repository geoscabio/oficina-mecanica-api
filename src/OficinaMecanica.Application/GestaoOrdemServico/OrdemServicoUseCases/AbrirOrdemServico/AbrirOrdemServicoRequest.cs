namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.AbrirOrdemServico;

public sealed record AbrirOrdemServicoRequest(Guid VeiculoId, Guid MecanicoId);
