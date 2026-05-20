namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ConsultarStatusOrdemServico;

public sealed record ConsultarStatusOrdemServicoResponse(Guid OrdemServicoId, int Numero, string Status, IReadOnlyCollection<ServicoStatusResponse> Servicos);

public sealed record ServicoStatusResponse(Guid ServicoId, Guid ServicoCatalogoId, string Status);
