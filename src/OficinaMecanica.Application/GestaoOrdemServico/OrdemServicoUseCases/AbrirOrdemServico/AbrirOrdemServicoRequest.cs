using OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.ReservarPecaInsumo;

namespace OficinaMecanica.Application.GestaoOrdemServico.OrdemServicoUseCases.AbrirOrdemServico;

public sealed record AbrirOrdemServicoRequest(
    Guid? ClienteId,
    string? DocumentoCliente,
    Guid VeiculoId,
    Guid MecanicoId,
    IReadOnlyCollection<Guid>? ServicosCatalogoIds,
    IReadOnlyCollection<PecaInsumoRequest>? PecasInsumos);
