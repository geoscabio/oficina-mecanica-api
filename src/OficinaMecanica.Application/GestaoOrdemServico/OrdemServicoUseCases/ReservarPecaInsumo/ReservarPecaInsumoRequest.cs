namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ReservarPecaInsumo;

public sealed record ReservarPecaInsumoRequest(Guid OrdemServicoId, IReadOnlyCollection<PecaInsumoRequest> PecasInsumos);

public sealed record PecaInsumoRequest(Guid PecaInsumoCatalogoId, int Quantidade);
